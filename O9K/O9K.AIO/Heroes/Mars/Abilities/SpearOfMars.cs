namespace O9K.AIO.Heroes.Mars.Abilities
{
    using System.Linq;

    using AIO.Abilities;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Base.Types;
    using Core.Extensions;
    using Core.Helpers;
    using Core.Managers.Entity;

    using Divine;
    using Divine.Camera;
    using Divine.Entity;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.GameConsole;

    using Divine.Input;
    using Divine.Log;
    using Divine.Map;

    using Divine.Modifier;
    using Divine.Numerics;
    using Divine.Orbwalker;
    using Divine.Order;
    using Divine.Particle;
    using Divine.Projectile;
    using Divine.Renderer;
    using Divine.Service;
    using Divine.Update;
    using Divine.Entity.Entities;
    using Divine.Entity.EventArgs;
    using Divine.Game.EventArgs;
    using Divine.GameConsole.Exceptions;
    using Divine.Input.EventArgs;
    using Divine.Map.Components;
    using Divine.Menu.Animations;
    using Divine.Menu.Components;

    using Divine.Menu.Helpers;

    using Divine.Menu.Styles;
    using Divine.Modifier.EventArgs;
    using Divine.Modifier.Modifiers;
    using Divine.Order.EventArgs;
    using Divine.Order.Orders;
    using Divine.Particle.Components;
    using Divine.Particle.EventArgs;
    using Divine.Particle.Particles;
    using Divine.Plugins.Humanizer;
    using Divine.Projectile.EventArgs;
    using Divine.Projectile.Projectiles;
    using Divine.Renderer.ValveTexture;
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Components;
    using Divine.Entity.Entities.EventArgs;
    using Divine.Entity.Entities.Exceptions;
    using Divine.Entity.Entities.PhysicalItems;
    using Divine.Entity.Entities.Players;
    using Divine.Entity.Entities.Runes;
    using Divine.Entity.Entities.Trees;
    using Divine.Entity.Entities.Units;
    using Divine.Modifier.Modifiers.Components;
    using Divine.Modifier.Modifiers.Exceptions;
    using Divine.Order.Orders.Components;
    using Divine.Particle.Particles.Exceptions;
    using Divine.Projectile.Projectiles.Components;
    using Divine.Projectile.Projectiles.Exceptions;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Abilities.Spells;
    using Divine.Entity.Entities.Players.Components;
    using Divine.Entity.Entities.Runes.Components;
    using Divine.Entity.Entities.Units.Buildings;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Entity.Entities.Units.Creeps;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Wards;
    using Divine.Entity.Entities.Abilities.Items.Components;
    using Divine.Entity.Entities.Abilities.Items.Neutrals;
    using Divine.Entity.Entities.Abilities.Spells.Abaddon;
    using Divine.Entity.Entities.Abilities.Spells.Components;
    using Divine.Entity.Entities.Units.Creeps.Neutrals;
    using Divine.Entity.Entities.Units.Heroes.Components;

    using Modes.Combo;

    using O9K.Core.Geometry;

    using Divine.Numerics;

    using TargetManager;

    internal class SpearOfMars : NukeAbility
    {
        private Vector3 castPosition;

        public SpearOfMars(ActiveAbility ability)
            : base(ability)
        {
        }

        public override bool CanHit(TargetManager targetManager, IComboModeMenu comboMenu)
        {
            var target = targetManager.Target;

            if (target.IsMagicImmune && !this.Ability.CanHitSpellImmuneEnemy)
            {
                return false;
            }

            if (this.Owner.Distance(target) > this.Ability.CastRange)
            {
                return false;
            }

            // ult
            if (target.HasModifier("modifier_mars_arena_of_blood_leash"))
            {
                return true;
            }

            var mainInput = this.Ability.GetPredictionInput(target);
            mainInput.Range = this.Ability.CastRange;
            var mainOutput = this.Ability.PredictionManager.GetSimplePrediction(mainInput);
            var targetPredictedPosition = mainOutput.TargetPosition;
            var range = this.Ability.Range - 200;
            var width = this.Ability.Radius;

            //todo predict collision positions ?
            var collisionRec = new Polygon.Rectangle(this.Owner.Position, targetPredictedPosition, width);
            if (targetManager.AllEnemyHeroes.Any(x => x != target && collisionRec.IsInside(x.Position)))
            {
                return false;
            }

            // tree
            foreach (var tree in EntityManager9.Trees)
            {
                var rec = new Polygon.Rectangle(
                    targetPredictedPosition,
                    this.Owner.Position.Extend2D(targetPredictedPosition, range),
                    width - 50);

                if (rec.IsInside(tree.Position))
                {
                    this.castPosition = mainOutput.CastPosition;
                    return true;
                }
            }

            // building
            foreach (var building in EntityManager9.Units.Where(x => x.IsBuilding && x.IsAlive))
            {
                var rec = new Polygon.Rectangle(
                    targetPredictedPosition,
                    this.Owner.Position.Extend2D(targetPredictedPosition, range),
                    width);

                if (rec.IsInside(building.Position))
                {
                    this.castPosition = mainOutput.CastPosition;
                    return true;
                }
            }

            // wall
            const int CellCount = 30;
            for (var i = 0; i < CellCount; ++i)
            {
                for (var j = 0; j < CellCount; ++j)
                {
                    var p = new Vector2(
                        (MapManager.MeshCellSize * (i - (CellCount / 2))) + targetPredictedPosition.X,
                        (MapManager.MeshCellSize * (j - (CellCount / 2))) + targetPredictedPosition.Y);

                    if ((MapManager.GetMeshCellFlags(p) & MeshCellFlags.InteractionBlocker) != 0)
                    {
                        var rec = new Polygon.Rectangle(
                            targetPredictedPosition,
                            this.Owner.Position.Extend2D(targetPredictedPosition, range),
                            width - 100);

                        if (rec.IsInside(p))
                        {
                            this.castPosition = mainOutput.CastPosition;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            if (this.castPosition.IsZero)
            {
                return base.UseAbility(targetManager, comboSleeper, aoe);
            }

            if (!this.Ability.UseAbility(this.castPosition))
            {
                return false;
            }

            this.castPosition = Vector3.Zero;

            var delay = this.Ability.GetCastDelay(targetManager.Target);
            if (this.Ability is IDisable disable)
            {
                targetManager.Target.SetExpectedUnitState(disable.AppliesUnitState, this.Ability.GetHitTime(targetManager.Target));
            }

            comboSleeper.Sleep(delay);
            this.Sleeper.Sleep(delay + 0.5f);
            this.OrbwalkSleeper.Sleep(delay);
            return true;
        }
    }
}
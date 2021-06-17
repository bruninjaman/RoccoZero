namespace O9K.AIO.Heroes.Pangolier.Abilities
{
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Abilities;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;

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

    using Divine.Numerics;

    using TargetManager;

    using BaseRollingThunder = Core.Entities.Abilities.Heroes.Pangolier.RollingThunder;

    internal class RollingThunder : NukeAbility
    {
        private readonly BaseRollingThunder rollingThunder;

        public RollingThunder(ActiveAbility ability)
            : base(ability)
        {
            this.rollingThunder = (BaseRollingThunder)ability;
        }

        public Vector3 GetPosition(Unit9 target)
        {
            var wallPositions = new List<Vector3>();

            var center = this.Owner.Position;
            const int CellCount = 40;
            for (var i = 0; i < CellCount; ++i)
            {
                for (var j = 0; j < CellCount; ++j)
                {
                    var p = new Vector2(
                        (MapManager.MeshCellSize * (i - (CellCount / 2))) + center.X,
                        (MapManager.MeshCellSize * (j - (CellCount / 2))) + center.Y);

                    if ((MapManager.GetMeshCellFlags(p) & MeshCellFlags.InteractionBlocker) != 0)
                    {
                        wallPositions.Add(new Vector3(p.X, p.Y, 0));
                    }
                }
            }

            return wallPositions.Where(this.CheckWall).OrderBy(x => this.Owner.Distance(x)).FirstOrDefault();
        }

        public override bool ShouldCast(TargetManager targetManager)
        {
            if (!base.ShouldCast(targetManager))
            {
                return false;
            }

            if (this.Owner.GetAngle(targetManager.Target.Position) > 0.75f)
            {
                return false;
            }

            return true;
        }

        public override bool ShouldConditionCast(TargetManager targetManager, IComboModeMenu menu, List<UsableAbility> usableAbilities)
        {
            var blink = usableAbilities.Find(x =>
                              x.Ability.Id == AbilityId.item_blink ||
                              x.Ability.Id == AbilityId.item_overwhelming_blink ||
                              x.Ability.Id == AbilityId.item_swift_blink ||
                              x.Ability.Id == AbilityId.item_arcane_blink);

            if (blink == null)
            {
                return this.Owner.Distance(targetManager.Target) < 600;
            }

            return this.Owner.Distance(targetManager.Target) < blink.Ability.CastRange + 200;
        }

        private bool CheckWall(Vector3 wall)
        {
            var distance = this.Owner.Distance(wall);
            // var turnTime = (this.rollingThunder.TurnRate * this.Owner.GetAngle(wall))- 0.75f;
            var turnTime = (this.Owner.GetAngle(wall) / this.rollingThunder.TurnRate) + 0.5f;

            if (turnTime > distance / this.rollingThunder.Speed)
            {
                return false;
            }

            if (distance > 600)
            {
                return false;
            }

            return true;
        }
    }
}
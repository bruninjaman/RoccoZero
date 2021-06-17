namespace O9K.AIO.Heroes.Timbersaw.Abilities
{
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Abilities;

    using Core.Entities.Abilities.Base;
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
    using Divine.Extensions;

    using Modes.Combo;

    using O9K.Core.Geometry;

    using Divine.Numerics;

    using TargetManager;

    using BaseTimberChain = Core.Entities.Abilities.Heroes.Timbersaw.TimberChain;

    internal class TimberChain : NukeAbility
    {
        private readonly BaseTimberChain timberChain;

        private Vector3 castPosition;

        public TimberChain(ActiveAbility ability)
            : base(ability)
        {
            this.timberChain = (BaseTimberChain)ability;
        }

        public override bool CanHit(TargetManager targetManager, IComboModeMenu comboMenu)
        {
            var target = targetManager.Target;
            var input = this.Ability.GetPredictionInput(target);
            input.Delay += this.Owner.Distance(target) / this.Ability.Speed;
            var output = this.Ability.GetPredictionOutput(input);
            var ownerPosition = this.Owner.Position;

            this.castPosition = output.CastPosition;

            var polygon = new Polygon.Rectangle(ownerPosition, this.castPosition, this.timberChain.ChainRadius);
            var availableTrees = EntityManager9.Trees.Where(x => x.Distance2D(ownerPosition) < this.Ability.CastRange).ToArray();

            foreach (var tree in availableTrees)
            {
                if (polygon.IsInside(tree.Position))
                {
                    return false;
                }
            }

            var chainEndPosition = ownerPosition.Extend2D(this.castPosition, this.Ability.CastRange);
            polygon = new Polygon.Rectangle(this.castPosition, chainEndPosition, this.timberChain.Radius);

            foreach (var tree in availableTrees)
            {
                if (polygon.IsInside(tree.Position))
                {
                    this.castPosition = tree.Position;
                    return true;
                }
            }

            if (this.Ability.Level < 4 || ownerPosition.Distance2D(this.castPosition) < 400)
            {
                return false;
            }

            foreach (var tree in availableTrees.OrderBy(x => x.Distance2D(this.castPosition)))
            {
                var treePosition = tree.Position;

                if (treePosition.Distance2D(this.castPosition) > 500 && target.GetAngle(treePosition) > 0.75f)
                {
                    continue;
                }

                if (ownerPosition.Distance2D(this.castPosition) < treePosition.Distance2D(this.castPosition))
                {
                    continue;
                }

                polygon = new Polygon.Rectangle(ownerPosition, treePosition, this.timberChain.ChainRadius);
                if (availableTrees.Any(x => !x.Equals(tree) && polygon.IsInside(x.Position)))
                {
                    continue;
                }

                this.castPosition = treePosition;
                return true;
            }

            return false;
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
                return true;
            }

            if (this.Owner.Distance(targetManager.Target) < 800)
            {
                return true;
            }

            return false;
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            if (!this.Ability.UseAbility(this.Owner.Position.Extend2D(this.castPosition, 400)))
            {
                return false;
            }

            var delay = this.Ability.GetCastDelay(this.castPosition);
            comboSleeper.Sleep(delay);
            this.Sleeper.Sleep(delay + 0.5f);
            this.OrbwalkSleeper.Sleep(this.Ability.GetHitTime(this.castPosition));
            return true;
        }
    }
}
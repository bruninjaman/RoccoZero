namespace O9K.AIO.Heroes.Dynamic.Abilities.Debuffs
{
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Modes.Combo;

    using Base;

    using Core.Entities.Abilities.Base.Components;
    using Core.Entities.Abilities.Base.Types;
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

    internal class DebuffAbilityGroup : OldAbilityGroup<IDebuff, OldDebuffAbility>
    {
        private readonly List<AbilityId> castOrderDown = new List<AbilityId>
        {
            // first
            AbilityId.leshrac_lightning_storm,
            AbilityId.item_dagon
            // last
        };

        private readonly List<AbilityId> castOrderUp = new List<AbilityId>
        {
            // last
            AbilityId.item_ethereal_blade,
            AbilityId.item_veil_of_discord,
            // fist
        };

        public DebuffAbilityGroup(BaseHero baseHero)
            : base(baseHero)
        {
        }

        public bool UseAmplifiers(Unit9 target, ComboModeMenu menu)
        {
            foreach (var ability in this.Abilities)
            {
                if (!(ability.Debuff is IHasDamageAmplify))
                {
                    continue;
                }

                if (!ability.Debuff.IsValid)
                {
                    continue;
                }

                if (!ability.CanBeCasted(target, menu))
                {
                    continue;
                }

                if (ability.Use(target))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void OrderAbilities()
        {
            this.Abilities = this.Abilities.OrderByDescending(x => this.castOrderUp.IndexOf(x.Debuff.Id))
                .ThenBy(x => this.castOrderDown.IndexOf(x.Debuff.Id))
                .ThenBy(x => x.Debuff.CastPoint)
                .ToList();
        }
    }
}
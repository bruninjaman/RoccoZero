namespace O9K.AIO.Heroes.Dynamic.Abilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using AIO.Modes.Combo;

    using Base;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Base.Components;
    using Core.Entities.Abilities.Base.Components.Base;
    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Helpers;

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

    internal class OldAbilityGroup<TType, TAbility> : IOldAbilityGroup
        where TType : class, IActiveAbility where TAbility : OldUsableAbility
    {
        public OldAbilityGroup(BaseHero baseHero)
        {
            foreach (var type in Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => !x.IsAbstract && x.IsClass && typeof(TAbility).IsAssignableFrom(x)))
            {
                foreach (var attribute in type.GetCustomAttributes<AbilityIdAttribute>())
                {
                    this.UniqueAbilities.Add(attribute.AbilityId, type);
                }
            }

            this.AbilitySleeper = baseHero.AbilitySleeper;
            this.OrbwalkSleeper = baseHero.OrbwalkSleeper;
        }

        public List<TAbility> Abilities { get; protected set; } = new List<TAbility>();

        protected MultiSleeper AbilitySleeper { get; }

        protected virtual HashSet<AbilityId> Ignored { get; } = new HashSet<AbilityId>();

        protected MultiSleeper OrbwalkSleeper { get; }

        protected Dictionary<AbilityId, Type> UniqueAbilities { get; } = new Dictionary<AbilityId, Type>();

        public virtual bool AddAbility(Ability9 ability)
        {
            if (this.IsIgnored(ability))
            {
                return false;
            }

            var type = ability as TType;
            if (type == null)
            {
                return false;
            }

            if (!this.UniqueAbilities.TryGetValue(ability.Id, out var uniqueType))
            {
                uniqueType = typeof(TAbility);
            }

            var usableAbility = (TAbility)Activator.CreateInstance(uniqueType, type);
            usableAbility.AbilitySleeper = this.AbilitySleeper;
            usableAbility.OrbwalkSleeper = this.OrbwalkSleeper;
            this.Abilities.Add(usableAbility);
            this.OrderAbilities();

            return true;
        }

        public bool CanBeCasted(HashSet<AbilityId> check, ComboModeMenu menu, Unit9 target)
        {
            foreach (var ability in this.Abilities.Where(x => check.Contains(x.Ability.Id)))
            {
                if (!ability.Ability.IsValid)
                {
                    continue;
                }

                if (ability.CanHit(target))
                {
                    continue;
                }

                if (!ability.CanBeCasted(menu))
                {
                    continue;
                }

                if (target.IsMagicImmune && !ability.Ability.PiercesMagicImmunity(target))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public void RemoveAbility(Ability9 ability)
        {
            var usableAbility = this.Abilities.Find(x => x.Ability.Handle == ability.Handle);
            if (usableAbility == null)
            {
                return;
            }

            if (usableAbility is IDisposable disposable)
            {
                disposable.Dispose();
            }

            this.Abilities.Remove(usableAbility);
        }

        public virtual bool Use(Unit9 target, ComboModeMenu menu, params AbilityId[] except)
        {
            foreach (var ability in this.Abilities)
            {
                if (!ability.Ability.IsValid)
                {
                    continue;
                }

                if (except.Contains(ability.Ability.Id))
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

        protected virtual bool IsIgnored(Ability9 ability)
        {
            return this.Ignored.Contains(ability.Id);
        }

        protected virtual void OrderAbilities()
        {
            this.Abilities = this.Abilities.OrderBy(x => x.Ability is IChanneled).ThenBy(x => x.Ability.CastPoint).ToList();
        }
    }
}
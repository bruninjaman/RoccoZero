using System.Linq;
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
using O9K.AIO.Heroes.Base;
using O9K.AIO.Modes.Combo;
using O9K.Core.Entities.Abilities.Heroes.Invoker.Helpers;

namespace O9K.AIO.Abilities
{
    internal class InvokerAbilityHelper : AbilityHelper
    {
        public InvokerAbilityHelper(TargetManager.TargetManager targetManager, IComboModeMenu comboModeMenu, ControllableUnit controllableUnit) : base(targetManager, comboModeMenu, controllableUnit)
        {
            
        }
        
        public bool UseInvokedAbilityIfCondition(UsableAbility ability, params UsableAbility[] checkAbilities)
        {
            if (!this.CanBeCasted(ability))
            {
                return false;
            }

            if (!IsInvoked(ability))
            {
                return false;
            }

            if (!ability.ShouldConditionCast(
                this.TargetManager,
                this.menu,
                checkAbilities.Where(x => this.CanBeCasted(x, false, false)).ToList()))
            {
                return false;
            }

            return ability.UseAbility(this.TargetManager, this.comboSleeper, true);
        }

        public bool IsInvoked(UsableAbility ability)
        {
            if (ability.Ability is IInvokableAbility invokable)
            {
                return invokable.IsInvoked;
            }

            return false;
        }

        public bool Invoke(UsableAbility ability)
        {
            if (ability.Ability is not IInvokableAbility ss) return false;
            return ss.Invoke(null, false, false, true);
        }

        public bool IsInvokedOnLastSlot(UsableAbility ability)
        {
            if (ability.Ability is IInvokableAbility invokable)
            {
                return invokable.GetAbilitySlot == AbilitySlot.Slot5;
            }

            return false;
        }

        public bool ReInvokeIfOnLastPosition(UsableAbility ability, params UsableAbility[] ignoredOnFirstSlot)
        {
            if (IsInvokedOnLastSlot(ability))
            {
                foreach (var usableAbility in ignoredOnFirstSlot.Where(x => x != null))
                {
                    if (usableAbility.Ability is IInvokableAbility usable)
                    {
                        if (usable.GetAbilitySlot == AbilitySlot.Slot4 && usableAbility.Ability.CanBeCasted())
                        {
                            return false;
                        }
                    }
                }

                if (ability.Ability is IInvokableAbility ss)
                {
                    return ss.Invoke(null, false, false, true);
                }
            }

            return false;
        }

        public bool SafeInvoke(UsableAbility ability, params UsableAbility[] ignoredOnInvokedSlots)
        {
            if (IsInvoked(ability))
            {
                return true;
            }
            foreach (var usableAbility in ignoredOnInvokedSlots)
            {
                if (usableAbility.Ability is not IInvokableAbility usable) continue;
                if (usable.GetAbilitySlot != AbilitySlot.Slot5 || !usableAbility.Ability.CanBeCasted()) continue;
                var anyOnFirst = ignoredOnInvokedSlots.Any(x =>
                {
                    if (x.Ability is not IInvokableAbility usable2) return false;
                    return usable2.GetAbilitySlot == AbilitySlot.Slot4 && usableAbility.Ability.CanBeCasted();
                });
                if (!anyOnFirst) continue;
                return false;
            }

            if (ability.Ability is not IInvokableAbility ss) return false;
            return ss.Invoke(null, false, false, true);
        }

        public bool CanBeInvoked(UsableAbility ability)
        {
            if (ability.Ability is IInvokableAbility invokable)
            {
                return invokable.CanBeInvoked;
            }

            return false;
        }
    }
}
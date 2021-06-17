namespace O9K.AIO.Heroes.Dynamic.Abilities.Specials.Unique
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Base.Components.Base;
    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Logger;

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

    [AbilityId(AbilityId.item_hurricane_pike)]
    internal class HurricanePikeSpecial : OldSpecialAbility, IDisposable
    {
        private readonly List<AbilityId> enabledOrbs = new List<AbilityId>();

        public HurricanePikeSpecial(IActiveAbility ability)
            : base(ability)
        {
        }

        public void Dispose()
        {
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
        }

        public override bool ShouldCast(Unit9 target)
        {
            if (target.IsLinkensProtected || target.IsSpellShieldProtected)
            {
                return false;
            }

            if (this.Ability.Owner.Distance(target) < target.GetAttackRange() + 100)
            {
                return true;
            }

            return false;
        }

        public override bool Use(Unit9 target)
        {
            if (!this.Ability.UseAbility(target))
            {
                return false;
            }

            ModifierManager.ModifierAdded += this.OnModifierAdded;

            this.OrbwalkSleeper.Sleep(this.Ability.Owner.Handle, this.Ability.GetCastDelay(target));
            this.AbilitySleeper.Sleep(this.Ability.Handle, this.Ability.GetHitTime(target) + 0.5f);

            return true;
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            if (modifier.Name != "modifier_item_hurricane_pike_range")
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                try
                {
                    if (modifier.Owner.Handle != this.Ability.Owner.Handle)
                    {
                        return;
                    }

                    foreach (var orb in this.Ability.Owner.Abilities.OfType<OrbAbility>())
                    {
                        if (orb.Enabled || !orb.CanBeCasted())
                        {
                            continue;
                        }

                        this.enabledOrbs.Add(orb.Id);
                        orb.Enabled = true;
                    }

                    ModifierManager.ModifierRemoved += this.OnModifierRemoved;
                    ModifierManager.ModifierAdded -= this.OnModifierAdded;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    ModifierManager.ModifierAdded -= this.OnModifierAdded;
                }
            });
        }

        private void OnModifierRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            if (modifier.Name != "modifier_item_hurricane_pike_range")
            {
                return;
            }

            try
            {
                if (modifier.Owner.Handle != this.Ability.Owner.Handle)
                {
                    return;
                }

                foreach (var orb in this.Ability.Owner.Abilities.OfType<OrbAbility>().Where(x => this.enabledOrbs.Contains(x.Id)))
                {
                    orb.Enabled = false;
                }

                ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
            }
            catch (Exception ex)
            {
                ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
                Logger.Error(ex);
            }
        }
    }
}
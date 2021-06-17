namespace O9K.AIO.Heroes.Earthshaker.Units
{
    using System;
    using System.Collections.Generic;

    using Abilities;

    using AIO.Abilities;
    using AIO.Abilities.Items;
    using AIO.Modes.Combo;

    using Base;

    using Core.Entities.Abilities.Base;
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

    using Modes;

    using TargetManager;

    [UnitName(nameof(HeroId.npc_dota_hero_earthshaker))]
    internal class Earthshaker : ControllableUnit
    {
        private BlinkDaggerShaker blink;

        private EchoSlam echo;

        private Fissure fissure;

        private ForceStaff force;

        private EnchantTotem totem;

        public Earthshaker(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.earthshaker_fissure, x => this.fissure = new Fissure(x) },
                { AbilityId.earthshaker_enchant_totem, x => this.totem = new EnchantTotem(x) },
                { AbilityId.earthshaker_echo_slam, x => this.echo = new EchoSlam(x) },

                { AbilityId.item_blink, x => this.blink = new BlinkDaggerShaker(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkDaggerShaker(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkDaggerShaker(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkDaggerShaker(x) },
                { AbilityId.item_force_staff, x => this.force = new ForceStaff(x) },
            };
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            switch (comboModeMenu)
            {
                case EarthshakerComboModeMenu shakerMenu when shakerMenu.PreferEnchantTotem:
                    return this.TotemCombo(targetManager, abilityHelper);
                case EarthshakerEchoSlamComboModeMenu _:
                    return this.EchoSlamCombo(targetManager, abilityHelper);
            }

            if (abilityHelper.UseAbility(this.echo))
            {
                return true;
            }

            if (abilityHelper.UseDoubleBlinkCombo(this.force, this.blink))
            {
                return true;
            }

            if (abilityHelper.UseAbilityIfCondition(this.blink, this.echo))
            {
                UpdateManager.BeginInvoke(111, () =>
                {
                    this.echo.ForceUseAbility(targetManager, this.ComboSleeper);
                    this.OrbwalkSleeper.ExtendSleep(0.1f);
                    this.ComboSleeper.ExtendSleep(0.1f);
                });
                return true;
            }

            if (abilityHelper.UseAbilityIfCondition(this.blink, this.totem))
            {
                UpdateManager.BeginInvoke(111, () =>
                {
                    this.totem.ForceUseAbility(targetManager, this.ComboSleeper);
                    this.OrbwalkSleeper.ExtendSleep(0.2f);
                    this.ComboSleeper.ExtendSleep(0.2f);
                });
                return true;
            }

            if (abilityHelper.UseAbility(this.force, 500, 100))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.fissure))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.totem))
            {
                return true;
            }

            return false;
        }

        public bool TotemCombo(TargetManager targetManager, AbilityHelper abilityHelper)
        {
            var distance = this.Owner.Distance(targetManager.Target);

            if (distance < 250 && this.Owner.HasModifier("modifier_earthshaker_enchant_totem"))
            {
                return false;
            }

            if (abilityHelper.UseAbility(this.totem))
            {
                return true;
            }

            if (abilityHelper.UseDoubleBlinkCombo(this.force, this.blink))
            {
                return true;
            }

            if (abilityHelper.UseAbilityIfCondition(this.blink, this.totem))
            {
                if (!this.Owner.HasModifier("modifier_earthshaker_enchant_totem"))
                {
                    UpdateManager.BeginInvoke(111, () =>
                    {
                        this.totem.ForceUseAbility(targetManager, this.ComboSleeper);
                        this.OrbwalkSleeper.ExtendSleep(0.2f);
                        this.ComboSleeper.ExtendSleep(0.2f);
                    });
                }
                else if (this.Owner.BaseUnit.Attack(targetManager.Target.BaseUnit))
                {
                    this.OrbwalkSleeper.ExtendSleep(0.1f);
                    this.ComboSleeper.ExtendSleep(0.1f);
                    return true;
                }

                return true;
            }

            if (abilityHelper.UseAbility(this.force, 500, 100))
            {
                return true;
            }

            if (!abilityHelper.CanBeCasted(this.totem) && (!abilityHelper.CanBeCasted(this.blink) || distance < 300))
            {
                if (abilityHelper.UseAbility(this.fissure))
                {
                    this.OrbwalkSleeper.ExtendSleep(0.1f);
                    return true;
                }
            }

            if (abilityHelper.UseAbility(this.echo))
            {
                return true;
            }

            return false;
        }

        private bool EchoSlamCombo(TargetManager targetManager, AbilityHelper abilityHelper)
        {
            if (abilityHelper.UseDoubleBlinkCombo(this.force, this.blink))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.echo))
            {
                this.ComboSleeper.ExtendSleep(0.1f);
                this.OrbwalkSleeper.ExtendSleep(0.1f);
                return true;
            }

            if (abilityHelper.UseAbilityIfCondition(this.blink, this.echo))
            {
                UpdateManager.BeginInvoke(111, () =>
                {
                    this.echo.ForceUseAbility(targetManager, this.ComboSleeper);
                    this.ComboSleeper.ExtendSleep(0.2f);
                    this.OrbwalkSleeper.ExtendSleep(0.2f);
                });
                return true;
            }

            if (abilityHelper.CanBeCasted(this.echo, false, false))
            {
                return false;
            }

            if (abilityHelper.UseAbility(this.totem))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.fissure))
            {
                return true;
            }

            return false;
        }
    }
}
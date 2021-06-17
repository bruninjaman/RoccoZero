namespace O9K.AIO.Heroes.Disruptor.Units
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
    using Core.Extensions;
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
using TargetManager;

    [UnitName(nameof(HeroId.npc_dota_hero_disruptor))]
    internal class Disruptor : ControllableUnit, IDisposable
    {
        private BlinkAbility blink;

        private KineticField field;

        private ForceStaff force;

        private TargetableAbility glimpse;

        private Particle glimpseParticle;

        private StaticStorm storm;

        private NukeAbility thunder;

        private DebuffAbility veil;

        public Disruptor(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.disruptor_thunder_strike, x => this.thunder = new NukeAbility(x) },
                { AbilityId.disruptor_glimpse, x => this.glimpse = new TargetableAbility(x) },
                { AbilityId.disruptor_kinetic_field, x => this.field = new KineticField(x) },
                { AbilityId.disruptor_static_storm, x => this.storm = new StaticStorm(x) },

                { AbilityId.item_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_force_staff, x => this.force = new ForceStaff(x) },
                { AbilityId.item_veil_of_discord, x => this.veil = new DebuffAbility(x) },
            };

            this.MoveComboAbilities.Add(AbilityId.disruptor_kinetic_field, _ => this.field);

            ParticleManager.ParticleAdded += OnParticleAdded;
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.UseAbility(this.glimpse))
            {
                this.blink?.Sleeper.Sleep(3f);
                this.force?.Sleeper.Sleep(3f);
                return true;
            }

            if (abilityHelper.CanBeCasted(this.glimpse, false))
            {
                if (abilityHelper.UseAbility(this.blink, this.glimpse.Ability.CastRange, this.glimpse.Ability.CastRange - 500))
                {
                    return true;
                }
            }
            else
            {
                if (abilityHelper.UseAbility(this.blink, 800, 500))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbility(this.force, this.glimpse.Ability.CastRange, this.glimpse.Ability.CastRange - 500))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.veil))
            {
                return true;
            }

            var glimpseTime = this.glimpse.Ability.TimeSinceCasted;
            if (glimpseTime < 1.8)
            {
                if (this.glimpseParticle?.IsValid == true && !targetManager.Target.IsMagicImmune)
                {
                    var position = this.glimpseParticle.GetControlPoint(1);

                    if (abilityHelper.CanBeCasted(this.field, false))
                    {
                        if (this.field.UseAbility(position, this.ComboSleeper))
                        {
                            return true;
                        }
                    }

                    var glimpseParticleTime = this.glimpseParticle.GetControlPoint(2).X;
                    if (glimpseTime + 0.35f > glimpseParticleTime && abilityHelper.CanBeCasted(this.storm, false))
                    {
                        if (this.storm.UseAbility(position, targetManager, this.ComboSleeper))
                        {
                            return true;
                        }
                    }

                    if (this.Owner.Distance(position) > this.storm.Ability.CastRange - 100)
                    {
                        this.Owner.BaseUnit.Move(position.Extend2D(this.Owner.Position, 500));
                        return true;
                    }
                }

                this.OrbwalkSleeper.Sleep(0.1f);
                return true;
            }

            if (glimpseTime > 2 && abilityHelper.UseAbilityIfNone(this.field, this.glimpse))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(this.storm))
            {
                var fieldCastTime = this.field.Ability.TimeSinceCasted;
                if (fieldCastTime <= 4)
                {
                    if (this.storm.UseAbility(this.field.CastPosition, targetManager, this.ComboSleeper))
                    {
                        return true;
                    }
                }
                else
                {
                    if (abilityHelper.UseAbility(this.storm))
                    {
                        return true;
                    }
                }
            }

            if (abilityHelper.CanBeCasted(this.thunder))
            {
                var mana = this.Owner.Mana - this.thunder.Ability.ManaCost;

                if (abilityHelper.CanBeCasted(this.field))
                {
                    mana -= this.field.Ability.ManaCost;
                }

                if (abilityHelper.CanBeCasted(this.storm))
                {
                    mana -= this.storm.Ability.ManaCost;
                }

                if (mana > 0)
                {
                    abilityHelper.UseAbility(this.thunder);
                }
            }

            return false;
        }

        public void Dispose()
        {
            ParticleManager.ParticleAdded -= OnParticleAdded;
        }

        protected override bool MoveComboUseDisables(AbilityHelper abilityHelper)
        {
            if (base.MoveComboUseDisables(abilityHelper))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.field))
            {
                return true;
            }

            return false;
        }

        private void OnParticleAdded(ParticleAddedEventArgs e)
        {
            if (e.Particle.Name == "particles/units/heroes/hero_disruptor/disruptor_glimpse_targetend.vpcf")
            {
                this.glimpseParticle = e.Particle;
            }
        }
    }
}
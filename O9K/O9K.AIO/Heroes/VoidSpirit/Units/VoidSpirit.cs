namespace O9K.AIO.Heroes.VoidSpirit.Units
{
    using System;
    using System.Collections.Generic;

    using Abilities;

    using AIO.Abilities;
    using AIO.Abilities.Items;

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

    using Modes.Combo;

    using TargetManager;

    [UnitName(nameof(HeroId.npc_dota_hero_void_spirit))]
    internal class VoidSpirit : ControllableUnit
    {
        private readonly Sleeper forceEul = new Sleeper();

        private readonly Sleeper forceRemnant = new Sleeper();

        private ShieldAbility bkb;

        private DisableAbility bloodthorn;

        private NukeAbility dagon;

        private Dissimilate dissimilate;

        private DisableAbility eul;

        private DisableAbility wind;

        private DisableAbility hex;

        private ShieldAbility mjollnir;

        private Nullifier nullifier;

        private DisableAbility orchid;

        private SpeedBuffAbility phase;

        private NukeAbility pulse;

        private DisableAbility remnant;

        private DebuffAbility shiva;

        private NukeAbility step;

        private DebuffAbility urn;

        private DebuffAbility veil;

        private DebuffAbility vessel;

        public VoidSpirit(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.void_spirit_aether_remnant, x => this.remnant = new AetherRemnant(x) },
                { AbilityId.void_spirit_dissimilate, x => this.dissimilate = new Dissimilate(x) },
                { AbilityId.void_spirit_resonant_pulse, x => this.pulse = new NukeAbility(x) },
                { AbilityId.void_spirit_astral_step, x => this.step = new NukeAbility(x) },

                { AbilityId.item_phase_boots, x => this.phase = new SpeedBuffAbility(x) },
                { AbilityId.item_black_king_bar, x => this.bkb = new ShieldAbility(x) },
                { AbilityId.item_mjollnir, x => this.mjollnir = new ShieldAbility(x) },
                { AbilityId.item_orchid, x => this.orchid = new DisableAbility(x) },
                { AbilityId.item_bloodthorn, x => this.bloodthorn = new Bloodthorn(x) },
                { AbilityId.item_shivas_guard, x => this.shiva = new DebuffAbility(x) },
                { AbilityId.item_veil_of_discord, x => this.veil = new DebuffAbility(x) },
                { AbilityId.item_cyclone, x => this.eul = new DisableAbility(x) },
                { AbilityId.item_wind_waker, x => this.eul = new DisableAbility(x) },
                { AbilityId.item_dagon_5, x => this.dagon = new NukeAbility(x) },
                { AbilityId.item_spirit_vessel, x => this.vessel = new DebuffAbility(x) },
                { AbilityId.item_urn_of_shadows, x => this.urn = new DebuffAbility(x) },
                { AbilityId.item_nullifier, x => this.nullifier = new Nullifier(x) },
                { AbilityId.item_sheepstick, x => this.hex = new DisableAbility(x) },
            };
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (this.Owner.HasModifier("modifier_void_spirit_dissimilate_phase"))
            {
                this.ComboSleeper.Sleep(0.1f);
                this.OrbwalkSleeper.Sleep(0.1f);
                return this.Owner.Move(targetManager.Target.Position);
            }

            if (abilityHelper.UseAbility(this.hex))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.nullifier))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.bloodthorn))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.orchid))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.veil))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.dagon))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.bkb, 500))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.mjollnir, 500))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.vessel))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.urn))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(this.remnant) && targetManager.Target.GetImmobilityDuration() <= 0)
            {
                if (abilityHelper.CanBeCasted(this.eul, false, false))
                {
                    if (abilityHelper.UseAbility(this.eul) || (this.forceEul && abilityHelper.ForceUseAbility(this.eul)))
                    {
                        this.forceRemnant.Sleep(0.5f);
                        this.forceEul.Reset();
                        return true;
                    }
                }
            }

            if (this.forceRemnant && abilityHelper.CanBeCasted(this.remnant, true, false)
                                  && abilityHelper.ForceUseAbility(this.remnant, true))
            {
                this.forceRemnant.Reset();
                return true;
            }

            if (this.Owner.Distance(targetManager.Target) > 300)
            {
                if (abilityHelper.UseAbility(this.step))
                {
                    this.forceEul.Sleep(0.5f);
                    return true;
                }
            }

            if (!abilityHelper.CanBeCasted(this.step, false) || this.Owner.Distance(targetManager.Target) < 500
                                                             || !targetManager.Target.CanMove())
            {
                if (abilityHelper.UseAbility(this.remnant))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbility(this.dissimilate))
            {
                return true;
            }

            if (abilityHelper.UseAbilityIfNone(this.shiva, this.dissimilate))
            {
                return true;
            }

            if (!this.Owner.HasAghanimsScepter || !targetManager.Target.IsSilenced)
            {
                if (abilityHelper.UseAbility(this.pulse))
                {
                    this.pulse.Sleeper.ExtendSleep(0.2f);
                    return true;
                }
            }

            if (abilityHelper.UseAbility(this.phase))
            {
                return true;
            }

            return false;
        }
    }
}
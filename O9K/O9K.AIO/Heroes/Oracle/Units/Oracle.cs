namespace O9K.AIO.Heroes.Oracle.Units
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

    using TargetManager;

    [UnitName(nameof(HeroId.npc_dota_hero_oracle))]
    internal class Oracle : ControllableUnit
    {
        private DisableAbility bloodthorn;

        private NukeAbility dagon;

        private ShieldAbility edict;

        private FortunesEnd end;

        private EtherealBlade ethereal;

        private PurifyingFlames flames;

        private DisableAbility hex;

        private Nullifier nullifier;

        private DisableAbility orchid;

        private ShieldAbility promise;

        private DebuffAbility urn;

        private DebuffAbility veil;

        private DebuffAbility vessel;

        public Oracle(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.oracle_fortunes_end, x => this.end = new FortunesEnd(x) },
                { AbilityId.oracle_fates_edict, x => this.edict = new ShieldAbility(x) },
                { AbilityId.oracle_purifying_flames, x => this.flames = new PurifyingFlames(x) },
                { AbilityId.oracle_false_promise, x => this.promise = new ShieldAbility(x) },

                { AbilityId.item_spirit_vessel, x => this.vessel = new DebuffAbility(x) },
                { AbilityId.item_urn_of_shadows, x => this.urn = new DebuffAbility(x) },
                { AbilityId.item_dagon_5, x => this.dagon = new NukeAbility(x) },
                { AbilityId.item_sheepstick, x => this.hex = new DisableAbility(x) },
                { AbilityId.item_veil_of_discord, x => this.veil = new DebuffAbility(x) },
                { AbilityId.item_orchid, x => this.orchid = new DisableAbility(x) },
                { AbilityId.item_ethereal_blade, x => this.ethereal = new EtherealBlade(x) },
                { AbilityId.item_bloodthorn, x => this.bloodthorn = new Bloodthorn(x) },
                { AbilityId.item_nullifier, x => this.nullifier = new Nullifier(x) },
            };
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.UseAbility(this.hex))
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

            if (abilityHelper.UseAbility(this.nullifier))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.veil))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.ethereal))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.dagon))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.end))
            {
                return true;
            }

            if (this.end.Ability.IsChanneling)
            {
                if (targetManager.Target.HasModifier("modifier_oracle_purifying_flames"))
                {
                    if (this.Owner.BaseUnit.Stop())
                    {
                        this.ComboSleeper.Sleep(0.1f);
                        return true;
                    }
                }
            }

            if (abilityHelper.CanBeCasted(this.flames, true, true, false))
            {
                if (this.end.Ability.IsChanneling && !this.end.FullChannelTime(comboModeMenu))
                {
                    if (abilityHelper.ForceUseAbility(this.flames))
                    {
                        return true;
                    }
                }
            }

            if (abilityHelper.UseAbility(this.flames))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.urn))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.vessel))
            {
                return true;
            }

            return false;
        }

        public void HealAllyCombo(TargetManager targetManager)
        {
            if (this.ComboSleeper.IsSleeping)
            {
                return;
            }

            if (!this.flames.Ability.CanBeCasted())
            {
                return;
            }

            var target = targetManager.Target;

            if (target.HasModifier(this.edict.Shield.ShieldModifierName, this.promise.Shield.ShieldModifierName))
            {
                this.flames.Ability.UseAbility(target);
                this.ComboSleeper.Sleep(this.flames.Ability.GetCastDelay(target));
                return;
            }

            if (this.edict.Ability.CanBeCasted())
            {
                this.edict.Ability.UseAbility(target);
                this.ComboSleeper.Sleep(this.edict.Ability.GetCastDelay(target));
            }
        }
    }
}
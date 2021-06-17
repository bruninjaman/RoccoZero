using System.Linq;

namespace O9K.AIO.Heroes.LegionCommander.Units
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

    [UnitName(nameof(HeroId.npc_dota_hero_legion_commander))]
    internal class LegionCommander : ControllableUnit
    {
        private DisableAbility abyssal;

        private BuffAbility armlet;

        private BuffAbility attack;

        private ShieldAbility bkb;

        private ShieldAbility bladeMail;

        private LegionBlink blink;

        private DisableAbility bloodthorn;

        private Duel duel;

        private DisableAbility halberd;

        private DebuffAbility medallion;

        private ShieldAbility mjollnir;

        private Nullifier nullifier;

        private NukeAbility odds;

        private DisableAbility orchid;

        private SpeedBuffAbility phase;

        private DebuffAbility solar;

        public LegionCommander(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper,
            ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.legion_commander_overwhelming_odds, x => this.odds = new OverwhelmingOdds(x) },
                { AbilityId.legion_commander_press_the_attack, x => this.attack = new BuffAbility(x) },
                { AbilityId.legion_commander_duel, x => this.duel = new Duel(x) },

                { AbilityId.item_blink, x => this.blink = new LegionBlink(x) },
                { AbilityId.item_swift_blink, x => this.blink = new LegionBlink(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new LegionBlink(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new LegionBlink(x) },
                { AbilityId.item_phase_boots, x => this.phase = new SpeedBuffAbility(x) },
                { AbilityId.item_blade_mail, x => this.bladeMail = new ShieldAbility(x) },
                { AbilityId.item_black_king_bar, x => this.bkb = new ShieldAbility(x) },
                { AbilityId.item_solar_crest, x => this.solar = new DebuffAbility(x) },
                { AbilityId.item_medallion_of_courage, x => this.medallion = new DebuffAbility(x) },
                { AbilityId.item_heavens_halberd, x => this.halberd = new DisableAbility(x) },
                { AbilityId.item_mjollnir, x => this.mjollnir = new ShieldAbility(x) },
                { AbilityId.item_armlet, x => this.armlet = new BuffAbility(x) },
                { AbilityId.item_abyssal_blade, x => this.abyssal = new DisableAbility(x) },
                { AbilityId.item_orchid, x => this.orchid = new DisableAbility(x) },
                { AbilityId.item_bloodthorn, x => this.bloodthorn = new Bloodthorn(x) },
                { AbilityId.item_nullifier, x => this.nullifier = new Nullifier(x) },
            };
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);
            var distance = this.Owner.Distance(targetManager.Target);

            if (abilityHelper.CanBeCasted(this.duel, false, false) &&
                ((distance <= 1400 && abilityHelper.CanBeCasted(this.blink))
                 || distance < 500))
            {
                if (abilityHelper.CanBeCasted(this.attack, false))
                {
                    if (abilityHelper.ForceUseAbility(this.attack))
                    {
                        return true;
                    }
                }

                if (abilityHelper.CanBeCasted(this.bladeMail, false))
                {
                    if (abilityHelper.ForceUseAbility(this.bladeMail))
                    {
                        return true;
                    }
                }

                if (abilityHelper.CanBeCasted(this.bkb, false))
                {
                    if (abilityHelper.ForceUseAbility(this.bkb))
                    {
                        return true;
                    }
                }

                if (abilityHelper.CanBeCasted(this.mjollnir, false))
                {
                    if (abilityHelper.ForceUseAbility(this.mjollnir))
                    {
                        return true;
                    }
                }

                if (abilityHelper.CanBeCasted(this.armlet, false))
                {
                    if (abilityHelper.ForceUseAbility(this.armlet))
                    {
                        return true;
                    }
                }

                if (!abilityHelper.CanBeCasted(this.duel) && abilityHelper.CanBeCasted(this.blink, false))
                {
                    if (abilityHelper.ForceUseAbility(this.blink))
                    {
                        return true;
                    }
                }
            }

            if (abilityHelper.UseAbility(this.abyssal))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.orchid))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.bloodthorn))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.nullifier))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.halberd))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.bkb, 500))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.bladeMail, 500))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.mjollnir, 400))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.medallion))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.solar))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.attack, 400))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.armlet, 300))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.duel))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.blink, 300, 0))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(this.odds) && !abilityHelper.CanBeCasted(this.duel) &&
                !abilityHelper.CanBeCasted(this.blink)
                && this.blink?.Sleeper.IsSleeping != true)
            {
                if (abilityHelper.UseAbility(this.odds))
                {
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
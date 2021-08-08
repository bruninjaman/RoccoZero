namespace O9K.AIO.Heroes.Meepo.Units
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

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Heroes.Components;

    using Modes.Combo;

    using TargetManager;

    [UnitName(nameof(HeroId.npc_dota_hero_meepo))]
    internal class Meepo : ControllableUnit
    {
        private DisableAbility earthbind;
        
        private PoofAbility poof;

        
        
        private DisableAbility abyssal;
        
        private ShieldAbility bkb;

        private ShieldAbility bladeMail;

        private BlinkAbility blink;

        private DisableAbility bloodthorn;

        private DisableAbility halberd;

        private DisableAbility hex;

        private DebuffAbility medallion;

        private Nullifier nullifier;

        private DisableAbility orchid;
        
        private DebuffAbility solar;


        public Meepo(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.meepo_earthbind, x => this.earthbind = new DisableAbility(x) },
                { AbilityId.meepo_poof, x => this.poof = new PoofAbility(x) },

                
                { AbilityId.item_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_orchid, x => this.orchid = new DisableAbility(x) },
                { AbilityId.item_nullifier, x => this.nullifier = new Nullifier(x) },
                { AbilityId.item_bloodthorn, x => this.bloodthorn = new Bloodthorn(x) },
                { AbilityId.item_blade_mail, x => this.bladeMail = new ShieldAbility(x) },
                { AbilityId.item_black_king_bar, x => this.bkb = new ShieldAbility(x) },
                { AbilityId.item_heavens_halberd, x => this.halberd = new DisableAbility(x) },
                { AbilityId.item_abyssal_blade, x => this.abyssal = new DisableAbility(x) },
                { AbilityId.item_solar_crest, x => this.solar = new DebuffAbility(x) },
                { AbilityId.item_medallion_of_courage, x => this.medallion = new DebuffAbility(x) },
                { AbilityId.item_sheepstick, x => this.hex = new DisableAbility(x) },
            };
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.UseAbility(this.bkb, 400))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.hex))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.abyssal))
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

            if (abilityHelper.UseAbility(this.halberd))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.solar))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.medallion))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.blink, 400, 0))
            {
                return true;
            }
            
            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("AIO.Meepo.Earthbind") && abilityHelper.UseAbility(this.earthbind))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("AIO.Meepo.Earthbind", 500);
                return true;
            }
            
            if (abilityHelper.UseAbility(this.poof))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.bladeMail, 400))
            {
                return true;
            }

            return false;
        }

        public override bool Orbwalk(Unit9 target, bool attack, bool move, ComboModeMenu comboMenu = null)
        {
            return base.Attack(target, comboMenu);
        }
    }
}
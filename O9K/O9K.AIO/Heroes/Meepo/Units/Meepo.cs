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
        private DisableAbility abyssal;

        private ShieldAbility bkb;

        private ShieldAbility bladeMail;

        private BlinkAbility blink;

        private DisableAbility bloodthorn;

        private DisableAbility earthbind;

        private DisableAbility halberd;

        private DisableAbility hex;

        private DebuffAbility medallion;

        private Nullifier nullifier;

        private DisableAbility orchid;

        private PoofAbility poof;

        private DebuffAbility solar;

        public Meepo(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.meepo_earthbind, x => earthbind = new DisableAbility(x) },
                { AbilityId.meepo_poof, x => poof = new PoofAbility(x) },

                { AbilityId.item_blink, x => blink = new BlinkAbility(x) },
                { AbilityId.item_swift_blink, x => blink = new BlinkAbility(x) },
                { AbilityId.item_arcane_blink, x => blink = new BlinkAbility(x) },
                { AbilityId.item_overwhelming_blink, x => blink = new BlinkAbility(x) },
                { AbilityId.item_orchid, x => orchid = new DisableAbility(x) },
                { AbilityId.item_nullifier, x => nullifier = new Nullifier(x) },
                { AbilityId.item_bloodthorn, x => bloodthorn = new Bloodthorn(x) },
                { AbilityId.item_blade_mail, x => bladeMail = new ShieldAbility(x) },
                { AbilityId.item_black_king_bar, x => bkb = new ShieldAbility(x) },
                { AbilityId.item_heavens_halberd, x => halberd = new DisableAbility(x) },
                { AbilityId.item_abyssal_blade, x => abyssal = new DisableAbility(x) },
                { AbilityId.item_solar_crest, x => solar = new DebuffAbility(x) },
                { AbilityId.item_medallion_of_courage, x => medallion = new DebuffAbility(x) },
                { AbilityId.item_sheepstick, x => hex = new DisableAbility(x) }
            };
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.UseAbility(bkb, 400))
                return true;

            if (abilityHelper.UseAbility(hex))
                return true;

            if (abilityHelper.UseAbility(abyssal))
                return true;

            if (abilityHelper.UseAbility(bloodthorn))
                return true;

            if (abilityHelper.UseAbility(orchid))
                return true;

            if (abilityHelper.UseAbility(nullifier))
                return true;

            if (abilityHelper.UseAbility(halberd))
                return true;

            if (abilityHelper.UseAbility(solar))
                return true;

            if (abilityHelper.UseAbility(medallion))
                return true;

            if (abilityHelper.UseAbility(blink, 400, 0))
                return true;

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("AIO.Meepo.Earthbind") && abilityHelper.UseAbility(earthbind))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("AIO.Meepo.Earthbind", 500);

                return true;
            }

            if (abilityHelper.UseAbility(poof))
                return true;

            if (abilityHelper.UseAbility(bladeMail, 400))
                return true;

            return false;
        }

        public override bool Orbwalk(Unit9 target, bool attack, bool move, ComboModeMenu comboMenu = null)
        {
            return base.Attack(target, comboMenu);
        }
    }
}
namespace O9K.AIO.Heroes.Axe.Units
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

    [UnitName(nameof(HeroId.npc_dota_hero_axe))]
    internal class Axe : ControllableUnit
    {
        private ShieldAbility bkb;

        private CullingBlade blade;

        private ShieldAbility bladeMail;

        private BlinkDaggerAOE blink;

        private DisableAbility call;

        private NukeAbility dagon;

        private ForceStaff force;

        private DebuffAbility hunger;

        private BuffAbility illusionCape;

        private BuffAbility manta;

        private DisableAbility meteor;

        private ShieldAbility mjollnir;

        private DebuffAbility shiva;

        public Axe(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.axe_berserkers_call, x => call = new DisableAbility(x) },
                { AbilityId.axe_battle_hunger, x => hunger = new DebuffAbility(x) },
                { AbilityId.axe_culling_blade, x => blade = new CullingBlade(x) },

                { AbilityId.item_blink, x => blink = new BlinkDaggerAOE(x) },
                { AbilityId.item_swift_blink, x => blink = new BlinkDaggerAOE(x) },
                { AbilityId.item_arcane_blink, x => blink = new BlinkDaggerAOE(x) },
                { AbilityId.item_overwhelming_blink, x => blink = new BlinkDaggerAOE(x) },
                { AbilityId.item_force_staff, x => force = new ForceStaff(x) },
                { AbilityId.item_blade_mail, x => bladeMail = new ShieldAbility(x) },
                { AbilityId.item_black_king_bar, x => bkb = new ShieldAbility(x) },
                { AbilityId.item_shivas_guard, x => shiva = new DebuffAbility(x) },
                { AbilityId.item_mjollnir, x => mjollnir = new ShieldAbility(x) },
                { AbilityId.item_meteor_hammer, x => meteor = new MeteorHammerAxe(x) },
                { AbilityId.item_dagon_5, x => dagon = new NukeAbility(x) },
                { AbilityId.item_manta, x => manta = new BuffAbility(x) },
                { AbilityId.item_illusionsts_cape, x => illusionCape = new BuffAbility(x) }
            };

            MoveComboAbilities.Add(AbilityId.axe_berserkers_call, _ => call);
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.UseAbility(bkb, 400))
            {
                return true;
            }

            if (abilityHelper.UseAbility(blade))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(blade, false) && !abilityHelper.CanBeCasted(blade))
            {
                if (abilityHelper.UseAbility(blink, 200, 0))
                {
                    return true;
                }
            }

            if (abilityHelper.UseDoubleBlinkCombo(force, blink))
            {
                return true;
            }

            if (abilityHelper.CanBeCastedIfCondition(blink, call))
            {
                if (abilityHelper.UseAbility(bkb))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbility(call))
            {
                call.Sleeper.ExtendSleep(1);

                return true;
            }

            if (abilityHelper.CanBeCasted(meteor))
            {
                if (targetManager.Target.HasModifier("modifier_axe_berserkers_call") && abilityHelper.UseAbility(meteor))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbilityIfCondition(blink, call))
            {
                return true;
            }

            if (abilityHelper.UseAbility(force, 500, 0))
            {
                return true;
            }

            if (!abilityHelper.CanBeCasted(call, false))
            {
                if (abilityHelper.UseAbility(bladeMail, 400))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbility(mjollnir, 400))
            {
                return true;
            }

            if (abilityHelper.UseAbility(shiva))
            {
                return true;
            }

            if (abilityHelper.UseAbility(dagon))
            {
                return true;
            }

            if (abilityHelper.UseAbility(manta, Owner.GetAttackRange()))
            {
                return true;
            }

            if (abilityHelper.UseAbility(illusionCape, Owner.GetAttackRange()))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(hunger))
            {
                if (abilityHelper.CanBeCasted(call))
                {
                    return false;
                }

                if (abilityHelper.CanBeCasted(meteor) && call?.Sleeper.IsSleeping == true)
                {
                    return false;
                }

                if (abilityHelper.UseAbility(hunger))
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool MoveComboUseDisables(AbilityHelper abilityHelper)
        {
            if (base.MoveComboUseDisables(abilityHelper))
            {
                return true;
            }

            if (abilityHelper.UseAbility(call))
            {
                return true;
            }

            return false;
        }
    }
}
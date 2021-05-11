namespace O9K.Core.Entities.Abilities.Heroes.Hoodwink
{
    using Base;

    using Divine;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Components;
    using O9K.Core.Entities.Abilities.Base.Types;
    using O9K.Core.Entities.Units;
    using O9K.Core.Helpers;
    using O9K.Core.Helpers.Damage;

    [AbilityId(AbilityId.hoodwink_acorn_shot)]
    public class AcornShot : RangedAbility, INuke, IDebuff, IAppliesImmobility
    {
        private bool talentLearned;

        private readonly SpecialData multiplierData;

        public AcornShot(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "bounce_range");
            this.DamageData = new SpecialData(baseAbility, "acorn_shot_damage");
            this.multiplierData = new SpecialData(baseAbility, "base_damage_pct");
        }

        public override bool IntelligenceAmplify { get; } = false;

        public override AbilityBehavior AbilityBehavior
        {
            get
            {
                return base.AbilityBehavior & ~AbilityBehavior.UnitTarget;
            }
        }

        public override bool IsDisplayingCharges
        {
            get
            {
                if (this.talentLearned)
                {
                    return true;
                }

                return this.talentLearned = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_hoodwink_acorn_shot_charges)?.Level > 0;
            }
        }

        public override Damage GetRawDamage(Unit9 unit, float? remainingHealth = null)
        {
            var damage = base.GetRawDamage(unit, remainingHealth);
            var autoAttackDamage = this.Owner.GetRawAttackDamage(unit);
            var multiplier = this.multiplierData.GetValue(this.Level) / 100;

            autoAttackDamage[this.DamageType] *= multiplier;

            return damage + autoAttackDamage;
        }

        public string DebuffModifierName { get; } = "modifier_hoodwink_acorn_shot_slow";

        public string ImmobilityModifierName { get; } = "modifier_hoodwink_acorn_shot_slow";
    }
}
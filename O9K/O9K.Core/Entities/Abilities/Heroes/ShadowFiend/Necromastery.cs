namespace O9K.Core.Entities.Abilities.Heroes.ShadowFiend
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Entities.Units;

    using Helpers;
    using Helpers.Damage;

    using Metadata;

    [AbilityId(AbilityId.nevermore_necromastery)]
    public class Necromastery : OrbAbility, INuke
    {
        public Necromastery(Ability baseAbility)
            : base(baseAbility)
        {
            this.RangeData = new SpecialData(baseAbility, "AbilityCastRange");
        }

        public override bool IsUsable
        {
            get
            {
                return this.Owner.HasAghanimShard;
            }
        }

        public override bool CanBeCasted(bool checkChanneling = true)
        {
            if (this.Owner.GetModifierStacks("modifier_nevermore_necromastery") == 0 || !this.IsUsable)
            {
                return false;
            }

            return base.CanBeCasted(checkChanneling);
        }

        public override AbilityBehavior AbilityBehavior { get; } = AbilityBehavior.UnitTarget;

        public override DamageType DamageType { get; } = DamageType.Physical;

        public override bool IntelligenceAmplify { get; } = false;

        public override Damage GetRawDamage(Unit9 unit, float? remainingHealth = null)
        {
            var crit = 1.7f;

            return this.Owner.GetRawAttackDamage(unit, DamageValue.Minimum, crit);
        }

    }
}
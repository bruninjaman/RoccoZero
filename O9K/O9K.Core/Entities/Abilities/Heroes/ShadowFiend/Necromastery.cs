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
    public class Necromastery : RangedAbility, INuke
    {
        public Necromastery(Ability baseAbility)
            : base(baseAbility)
        {
            this.RangeData = new SpecialData(baseAbility, "AbilityCastRange");
        }

        public override float CastPoint
        {
            get
            {
                return this.Owner.GetAttackPoint();
            }
        }

        public override bool IsUsable
        {
            get
            {
                if (!this.Owner.HasAghanimShard)
                {
                    return  false;
                }

                return base.IsUsable;
            }
        }

        public override AbilityBehavior AbilityBehavior
        {
            get
            {
                return AbilityBehavior.UnitTarget;
            }
        }

        public override float CastRange
        {
            get
            {
                return this.RangeData.GetValue(this.Level);
            }
        }

        public override DamageType DamageType { get; } = DamageType.Physical;

        public override bool IntelligenceAmplify { get; } = false;

        public override Damage GetRawDamage(Unit9 unit, float? remainingHealth = null)
        {
            var crit = 1.7f;

            return this.Owner.GetRawAttackDamage(unit, DamageValue.Minimum, crit);
        }
    }
}
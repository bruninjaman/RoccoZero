namespace O9K.Core.Entities.Abilities.Heroes.Broodmother
{
    using Base;

    using Divine;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Types;
    using O9K.Core.Helpers;

    [AbilityId(AbilityId.broodmother_silken_bola)]
    public class SilkenBola : RangedAbility, INuke, IDebuff
    {
        public SilkenBola(Ability baseAbility)
            : base(baseAbility)
        {
            this.SpeedData = new SpecialData(baseAbility, "projectile_speed");
            this.DamageData = new SpecialData(baseAbility, "impact_damage");
        }

        public string DebuffModifierName { get; } = "modifier_broodmother_silken_bola";

        public override DamageType DamageType { get; } = DamageType.Magical;

        public override AbilityBehavior AbilityBehavior
        {
            get
            {
                var behavior = base.AbilityBehavior;

                if (this.Owner.HasModifier("modifier_item_aghanims_shard"))
                {
                    behavior = (behavior & ~AbilityBehavior.UnitTarget) | AbilityBehavior.Point;
                }

                return behavior;
            }
        }

        public override float Radius
        {
            get
            {
                return 550;
            }
        }
    }
}
namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.item_fallen_sky)]
    public class FallenSky : CircleAbility, IDisable, IBlink
    {
        public FallenSky(Ability baseAbility)
            : base(baseAbility)
        {
            this.ActivationDelayData = new SpecialData(baseAbility, "land_time");
            this.RadiusData = new SpecialData(baseAbility, "impact_radius");
            this.DamageData = new SpecialData(baseAbility, "impact_damage_units");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Stunned;

        public BlinkType BlinkType { get; } = BlinkType.Blink;
    }
}
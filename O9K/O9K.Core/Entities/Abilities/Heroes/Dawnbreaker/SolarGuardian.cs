namespace O9K.Core.Entities.Abilities.Heroes.DarkWillow
{
    using Base;

    using Divine;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Types;
    using O9K.Core.Helpers;

    [AbilityId(AbilityId.dawnbreaker_solar_guardian)]
    public class SolarGuardian : RangedAreaOfEffectAbility, IDisable
    {
        public SolarGuardian(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
            this.DamageData = new SpecialData(baseAbility, "land_damage");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Stunned;

        public override float CastRange { get; } = 9999999;
    }
}
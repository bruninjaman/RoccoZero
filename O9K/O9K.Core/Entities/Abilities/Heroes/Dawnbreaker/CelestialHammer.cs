namespace O9K.Core.Entities.Abilities.Heroes.DarkWillow
{
    using Base;

    using Divine;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Types;
    using O9K.Core.Helpers;

    [AbilityId(AbilityId.dawnbreaker_celestial_hammer)]
    public class CelestialHammer : LineAbility, INuke
    {
        private readonly SpecialData castRangeData;

        private readonly SpecialData castRangeBonusData;

        public CelestialHammer(Ability baseAbility)
            : base(baseAbility)
        {
            this.castRangeData = new SpecialData(baseAbility, "range");
            this.castRangeBonusData = new SpecialData(baseAbility.Owner, AbilityId.special_bonus_unique_dawnbreaker_celestial_hammer_cast_range);
            this.RadiusData = new SpecialData(baseAbility, "hammer_aoe_radius");
            this.DamageData = new SpecialData(baseAbility, "hammer_damage");
            this.SpeedData = new SpecialData(baseAbility, "projectile_speed");
        }

        protected override float BaseCastRange
        {
            get
            {
                return this.castRangeData.GetValue(this.Level) + castRangeBonusData.GetValue(0);
            }
        }
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.NagaSiren
{
    using Base;

    using Divine;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.naga_siren_rip_tide)]
    public class RipTide : PassiveAbility
    {
        public RipTide(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
            this.DamageData = new SpecialData(baseAbility, "damage");
        }
    }
}
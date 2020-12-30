namespace O9K.Core.Entities.Abilities.Heroes.Luna
{
    using Base;

    using Divine;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.luna_eclipse)]
    public class Eclipse : AreaOfEffectAbility
    {
        public Eclipse(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
        }
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.Disruptor
{
    using Base;

    using Divine;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.disruptor_kinetic_field)]
    public class KineticField : CircleAbility
    {
        public KineticField(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
            this.ActivationDelayData = new SpecialData(baseAbility, "formation_time");
        }
    }
}
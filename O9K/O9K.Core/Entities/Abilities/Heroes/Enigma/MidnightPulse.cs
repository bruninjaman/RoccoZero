namespace O9K.Core.Entities.Abilities.Heroes.Enigma
{
    using Base;

    using Divine;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.enigma_midnight_pulse)]
    public class MidnightPulse : CircleAbility
    {
        public MidnightPulse(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
        }
    }
}
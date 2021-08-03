namespace O9K.Core.Entities.Abilities.Heroes.Brewmaster
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.brewmaster_void_astral_pulse)]
    public class ResonantPulse : AreaOfEffectAbility, IDebuff
    {
        public ResonantPulse(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
        }

        public string DebuffModifierName { get; } = "modifier_brewmaster_void_astral_pulse";
    }
}
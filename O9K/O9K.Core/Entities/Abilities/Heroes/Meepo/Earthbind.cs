namespace O9K.Core.Entities.Abilities.Heroes.Meepo
{
    using Base;
    using Base.Components;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.meepo_earthbind)]
    public class Earthbind : CircleAbility, IDisable, IAppliesImmobility
    {
        public Earthbind(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
            this.SpeedData = new SpecialData(baseAbility, "speed");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Rooted;

        public string ImmobilityModifierName { get; } = "modifier_meepo_earthbind";
    }
}
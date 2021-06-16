namespace O9K.Core.Entities.Abilities.Heroes.WitchDoctor
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.witch_doctor_paralyzing_cask)]
    public class ParalyzingCask : RangedAbility, IDisable
    {
        public ParalyzingCask(Ability baseAbility)
            : base(baseAbility)
        {
            this.SpeedData = new SpecialData(baseAbility, "speed");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Stunned;
    }
}
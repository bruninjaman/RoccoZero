namespace O9K.Core.Entities.Abilities.Heroes.Enigma
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Metadata;

    [AbilityId(AbilityId.enigma_malefice)]
    public class Malefice : RangedAbility, IDisable
    {
        public Malefice(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public UnitState AppliesUnitState { get; } = UnitState.Stunned;
    }
}
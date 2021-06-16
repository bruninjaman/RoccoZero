namespace O9K.Core.Entities.Abilities.Heroes.Morphling
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.morphling_adaptive_strike_str)]
    public class AdaptiveStrikeStrength : RangedAbility, IDisable
    {
        public AdaptiveStrikeStrength(Ability baseAbility)
            : base(baseAbility)
        {
            this.SpeedData = new SpecialData(baseAbility, "projectile_speed");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Stunned;
    }
}
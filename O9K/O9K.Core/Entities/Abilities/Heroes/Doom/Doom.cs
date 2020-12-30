namespace O9K.Core.Entities.Abilities.Heroes.Doom
{
    using Base;
    using Base.Types;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.doom_bringer_doom)]
    public class Doom : RangedAbility, IDisable
    {
        public Doom(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public UnitState AppliesUnitState { get; } = UnitState.Silenced | UnitState.Muted;
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.Hoodwink
{
    using Base;

    using Divine;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Types;

    [AbilityId(AbilityId.hoodwink_bushwhack)]
    public class Bushwhack : RangedAreaOfEffectAbility, IDisable
    {
        public Bushwhack(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public UnitState AppliesUnitState { get; } = UnitState.Stunned;
    }
}
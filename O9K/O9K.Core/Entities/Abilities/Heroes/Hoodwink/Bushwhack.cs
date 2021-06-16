namespace O9K.Core.Entities.Abilities.Heroes.Hoodwink
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

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
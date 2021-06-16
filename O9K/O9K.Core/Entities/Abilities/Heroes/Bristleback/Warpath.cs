namespace O9K.Core.Entities.Abilities.Heroes.Bristleback
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.bristleback_warpath)]
    public class Warpath : PassiveAbility
    {
        public Warpath(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
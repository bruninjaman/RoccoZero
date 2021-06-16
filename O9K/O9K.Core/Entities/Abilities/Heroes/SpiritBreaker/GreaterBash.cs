namespace O9K.Core.Entities.Abilities.Heroes.SpiritBreaker
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.spirit_breaker_greater_bash)]
    public class GreaterBash : PassiveAbility
    {
        public GreaterBash(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.Disruptor
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.disruptor_glimpse)]
    public class Glimpse : RangedAbility
    {
        public Glimpse(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
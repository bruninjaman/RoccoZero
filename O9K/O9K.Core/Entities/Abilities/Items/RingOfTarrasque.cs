namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    //todo id
    [AbilityId((AbilityId)279)]
    internal class RingOfTarrasque : PassiveAbility
    {
        public RingOfTarrasque(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
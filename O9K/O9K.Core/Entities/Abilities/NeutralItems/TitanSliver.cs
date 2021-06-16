namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_titan_sliver)]
    public class TitanSliver : PassiveAbility
    {
        public TitanSliver(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
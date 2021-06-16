namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_mysterious_hat)]
    public class FairysTrinket : PassiveAbility
    {
        public FairysTrinket(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
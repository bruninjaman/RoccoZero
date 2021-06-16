namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_vladmir)]
    public class VladmirsOffering : PassiveAbility
    {
        public VladmirsOffering(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
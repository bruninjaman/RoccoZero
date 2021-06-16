namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_eagle)]
    public class Eaglesong : PassiveAbility
    {
        public Eaglesong(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
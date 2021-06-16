namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_platemail)]
    public class Platemail : PassiveAbility
    {
        public Platemail(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
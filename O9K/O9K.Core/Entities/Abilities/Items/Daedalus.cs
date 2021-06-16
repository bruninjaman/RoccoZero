namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_greater_crit)]
    public class Daedalus : PassiveAbility
    {
        public Daedalus(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
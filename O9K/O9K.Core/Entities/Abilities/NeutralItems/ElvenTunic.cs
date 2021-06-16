namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_elven_tunic)]
    public class ElvenTunic : PassiveAbility
    {
        public ElvenTunic(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
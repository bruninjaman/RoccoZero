namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

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
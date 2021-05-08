namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_ceremonial_robe)]
    public class CeremonialRobe : PassiveAbility
    {
        public CeremonialRobe(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
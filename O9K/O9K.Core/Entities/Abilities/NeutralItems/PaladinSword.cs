namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_paladin_sword)]
    public class PaladinSword : PassiveAbility
    {
        public PaladinSword(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
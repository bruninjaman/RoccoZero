namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_spell_prism)]
    public class SpellPrism : PassiveAbility
    {
        public SpellPrism(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
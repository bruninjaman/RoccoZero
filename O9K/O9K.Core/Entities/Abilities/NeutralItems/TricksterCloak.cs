namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;
    using Base.Types;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_trickster_cloak)]
    public class TricksterCloak : ActiveAbility, IBuff
    {
        public TricksterCloak(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public string BuffModifierName { get; } = "modifier_item_trickster_cloak_invis";

        public bool BuffsAlly { get; } = false;

        public bool BuffsOwner { get; } = true;

        public override bool IsInvisibility { get; } = true;
    }
}
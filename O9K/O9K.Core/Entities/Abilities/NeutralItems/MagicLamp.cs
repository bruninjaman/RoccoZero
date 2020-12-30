namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_panic_button)]
    public class MagicLamp : PassiveAbility
    {
        public MagicLamp(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
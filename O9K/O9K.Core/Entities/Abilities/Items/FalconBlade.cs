namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_falcon_blade)]
    public class FalconBlade : PassiveAbility
    {
        public FalconBlade(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
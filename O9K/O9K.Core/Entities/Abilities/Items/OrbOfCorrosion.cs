namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_orb_of_corrosion)]
    public class OrbOfCorrosion : PassiveAbility
    {
        public OrbOfCorrosion(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
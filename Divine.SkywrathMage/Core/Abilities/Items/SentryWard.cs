using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ward_sentry)]
    public sealed class SentryWard : RangedItem
    {
        public SentryWard(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.ProvidesVision;
    }
}
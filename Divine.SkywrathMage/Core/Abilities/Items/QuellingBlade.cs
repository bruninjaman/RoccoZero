using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_quelling_blade)]
    public sealed class QuellingBlade : RangedItem
    {
        public QuellingBlade(Item item)
            : base(item)
        {
        }

        public float CastRangeOnWard
        {
            get
            {
                return GetAbilitySpecialData("cast_range_ward");
            }
        }
    }
}
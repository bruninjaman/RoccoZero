using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_bfury)]
    public sealed class BattleFury : RangedItem //TODO DamageIncrease
    {
        public BattleFury(Item item)
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
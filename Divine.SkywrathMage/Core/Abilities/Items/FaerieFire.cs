using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_faerie_fire)]
    public sealed class FaerieFire : ActiveItem, IHasHealthRestore
    {
        public FaerieFire(Item item)
            : base(item)
        {
        }

        public float TotalHealthRestore
        {
            get
            {
                return GetAbilitySpecialData("hp_restore");
            }
        }
    }
}
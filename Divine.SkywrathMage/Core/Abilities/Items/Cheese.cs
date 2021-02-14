using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_cheese)]
    public sealed class Cheese : ActiveItem, IHasHealthRestore, IHasManaRestore
    {
        public Cheese(Item item)
            : base(item)
        {
        }

        public float TotalHealthRestore
        {
            get
            {
                return GetAbilitySpecialData("health_restore");
            }
        }

        public float TotalManaRestore
        {
            get
            {
                return GetAbilitySpecialData("mana_restore");
            }
        }
    }
}
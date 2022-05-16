using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_sphere)]
    public sealed class LinkensSphere : RangedItem, IHasTargetModifier
    {
        public LinkensSphere(Item item)
            : base(item)
        {
        }

        internal override void Dispose()
        {
            base.Dispose();

            Owner.LinkensSphere = null;
        }

        private CUnit owner;

        public override CUnit Owner
        {
            get
            {
                return owner;
            }

            internal set
            {
                owner = value;
                owner.LinkensSphere = this;
            }
        }

        public string TargetModifierName { get; } = "modifier_item_sphere_target";
    }
}
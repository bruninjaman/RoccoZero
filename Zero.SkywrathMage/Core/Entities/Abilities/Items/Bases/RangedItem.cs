using System.Linq;

using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class RangedItem : ActiveItem
    {
        protected RangedItem(Item item)
            : base(item)
        {
        }

        public override float CastRange
        {
            get
            {
                var bonusRange = 0.0f;

                var talent = Owner.Spells.FirstOrDefault(x => x.Level > 0 && x.Name.StartsWith("special_bonus_cast_range_"));
                if (talent != null)
                {
                    bonusRange += talent.GetAbilitySpecialData("value");
                }

                var aetherLens = Owner.GetItemById(AbilityId.item_aether_lens);
                if (aetherLens != null)
                {
                    bonusRange += aetherLens.GetAbilitySpecialData("cast_range_bonus");
                }

                return BaseCastRange + bonusRange;
            }
        }
    }
}
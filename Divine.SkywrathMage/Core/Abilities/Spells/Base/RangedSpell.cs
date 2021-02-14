using System.Linq;

using Divine.SDK.Extensions;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class RangedSpell : ActiveSpell
    {
        protected RangedSpell(Ability ability)
            : base(ability)
        {
        }

        public override float CastRange
        {
            get
            {
                var bonusRange = 0.0f;

                var talent = Owner.Spellbook.Talents.FirstOrDefault(x => x.Level > 0 && x.Name.StartsWith("special_bonus_cast_range_"));
                if (talent != null)
                {
                    bonusRange += talent.GetAbilitySpecialData("value");
                }

                var aetherLens = Owner.GetItemById(AbilityId.item_aether_lens);
                if (aetherLens != null)
                {
                    bonusRange += aetherLens.GetAbilitySpecialData("cast_range_bonus");
                }

                var neutralItem = Owner.Inventory.NeutralItem;
                if (neutralItem != null && (neutralItem.Id == AbilityId.item_seer_stone || neutralItem.Id == AbilityId.item_keen_optic))
                {
                    bonusRange += neutralItem.GetAbilitySpecialData("cast_range_bonus");
                }

                return BaseCastRange + bonusRange;
            }
        }
    }
}
namespace Ensage.SDK.Abilities
{
    using System.Linq;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Extensions;

    public abstract class RangedAbility : ActiveAbility
    {
        protected RangedAbility(Ability ability)
            : base(ability)
        {
        }

        public override float CastRange
        {
            get
            {
                var bonusRange = 0.0f;

                var talent = this.Owner.Spellbook.Spells.FirstOrDefault(x => x.Level > 0 && x.Name.StartsWith("special_bonus_cast_range_"));
                if (talent != null)
                {
                    bonusRange += talent.GetAbilitySpecialData("value");
                }

                var aetherLens = this.Owner.GetItemById(AbilityId.item_aether_lens);
                if (aetherLens != null)
                {
                    bonusRange += aetherLens.GetAbilitySpecialData("cast_range_bonus");
                }

                return this.BaseCastRange + bonusRange;
            }
        }

        protected override float BaseCastRange
        {
            get
            {
                return this.Ability.CastRange;
            }
        }
    }
}
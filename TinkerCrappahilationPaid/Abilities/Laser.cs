using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Extensions;

namespace TinkerCrappahilationPaid.Abilities
{
    public class Laser : RangedAbility
    {
        public Laser(Ability ability) : base(ability)
        {

        }


        protected override float RawDamage
        {
            get
            {
                var damage = Ability.GetAbilitySpecialData("laser_damage");

                var talent = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_tinker);
                if (talent?.Level > 0)
                {
                    damage += talent.GetAbilitySpecialData("value");
                }
                return damage;
            }
        }
    }
}
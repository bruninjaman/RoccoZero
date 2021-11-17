using Divine.Entity;
using Divine.Entity.Entities.Abilities;
using Divine.Extensions;

namespace TinkerEW
{
    internal static class AbilityExstensions
    {
        public static bool CanBeCasted(this Ability item)
        {
            var localHero = EntityManager.LocalHero;

            if (localHero == null)
                return false;

            return !item.IsInAbilityPhase
            && !localHero.IsChanneling()
            && item.Level > 0
            && !localHero.IsHexed()
            && !localHero.IsSilenced()
            && !localHero.IsStunned()
            && item.Cooldown <= 0f
            && localHero.Mana >= item.ManaCost;
        }
    }
}
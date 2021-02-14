using System.Linq;

using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Spells.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Helpers;
using Divine.SDK.Extensions;

namespace Divine.Core.Entities.Abilities.Spells.SkywrathMage
{
    [Spell(AbilityId.skywrath_mage_mystic_flare, HeroId.npc_dota_hero_skywrath_mage)]
    public sealed class MysticFlare : CircleSpell, IHasDot
    {
        public MysticFlare(Ability ability)
            : base(ability)
        {
        }

        public float DamageDuration
        {
            get
            {
                return GetAbilitySpecialData("duration");
            }
        }

        public bool HasInitialDamage { get; } = false;

        public float RawTickDamage
        {
            get
            {
                var damage = GetAbilitySpecialData("damage");

                var talent = Owner.GetAbilityById(AbilityId.special_bonus_unique_skywrath_5);
                if (talent?.Level > 0)
                {
                    damage += talent.GetAbilitySpecialData("value");
                }

                return (damage / DamageDuration) * TickRate;
            }
        }

        public string TargetModifierName { get; } = "modifier_skywrath_mystic_flare_aura_effect";

        public float TickRate
        {
            get
            {
                return GetAbilitySpecialData("damage_interval");
            }
        }

        public float GetTickDamage(params Unit[] targets)
        {
            var damage = RawTickDamage;
            var amplify = this.SpellAmplification();
            if (!targets.Any())
            {
                return DamageHelpers.GetSpellDamage(damage, amplify);
            }

            damage /= targets.Length;

            var totalDamage = 0.0f;
            foreach (var target in targets)
            {
                var reduction = this.GetDamageReduction(target, DamageType);
                totalDamage += DamageHelpers.GetSpellDamage(damage, amplify, reduction);
            }

            return totalDamage;
        }

        public float GetTotalDamage(params Unit[] targets)
        {
            return GetTickDamage(targets) * (DamageDuration / TickRate);
        }
    }
}
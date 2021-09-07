namespace O9K.Core.Entities.Heroes.Unique;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;

using Managers.Entity;

using Metadata;

[HeroId(HeroId.npc_dota_hero_vengefulspirit)]
internal class VengefulSpirit : Hero9
{
    public VengefulSpirit(Hero baseHero)
        : base(baseHero)
    {
        if (this.IsIllusion)
        {
            if (!this.HasModifier("modifier_vengefulspirit_hybrid_special"))
            {
                return;
            }

            var mainHero = EntityManager9.GetUnitFast(baseHero.ReplicateFrom?.Handle);
            if (mainHero == null)
            {
                return;
            }

            this.CanUseAbilities = mainHero.GetAbilityById(AbilityId.vengefulspirit_command_aura)?.Level > 0 && mainHero.HasAghanimsScepter;
        }
    }
}
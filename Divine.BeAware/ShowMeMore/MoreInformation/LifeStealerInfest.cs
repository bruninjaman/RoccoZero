using System.Linq;
using System.Threading.Tasks;

using Divine.BeAware.MenuManager.ShowMeMore;
using Divine.SDK.Extensions;

namespace Divine.BeAware.ShowMeMore.MoreInformation
{
    internal sealed class LifeStealerInfest : Base
    {
        public LifeStealerInfest(Common common) : base(common)
        {
        }

        public override bool Modifier(Unit unit, Modifier modifier, bool isHero)
        {
            if (modifier.Name != "modifier_life_stealer_infest_effect")
            {
                return false;
            }

            if (!MoreInformationMenu.LifeStealerInfestItem)
            {
                return true;
            }

            Infest(unit, modifier, isHero);
            return true;
        }

        private async void Infest(Unit unit, Modifier modifier, bool isHero)
        {
            if (EntityManager.GetEntities<Hero>().Any(x => x.IsAlly(LocalHero) && !x.IsIllusion && x.HeroId == HeroId.npc_dota_hero_life_stealer))
            {
                return;
            }

            var effectName = "particles/units/heroes/hero_life_stealer/life_stealer_infested_unit.vpcf";
            if (isHero)
            {
                effectName = "materials/ensage_ui/particles/life_stealer_infested_unit.vpcf";
            }

            ParticleManager.CreateOrUpdateParticle($"LifeStealerInfest_{unit.Handle}", effectName, EntityManager.GetEntityByHandle(unit.Handle), ParticleAttachment.OverheadFollow);

            while (modifier.IsValid)
            {
                await Task.Delay(150);
            }

            ParticleManager.RemoveParticle($"LifeStealerInfest_{unit.Handle}");
        }
    }
}
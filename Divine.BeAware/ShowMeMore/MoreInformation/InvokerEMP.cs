using System.Linq;

using Divine.BeAware.MenuManager.ShowMeMore;
using Divine.BeAware.MenuManager.ShowMeMore.MoreInformation;
using Divine.SDK.Extensions;
using Divine.SDK.Managers.Update;

using SharpDX;

namespace Divine.BeAware.ShowMeMore.MoreInformation
{
    internal sealed class InvokerEMP : Base
    {
        private readonly InvokerEMPMenu InvokerEMPMenu;

        public InvokerEMP(Common common) : base(common)
        {
            InvokerEMPMenu = MoreInformationMenu.InvokerEMPMenu;
        }

        private Color Color
        {
            get
            {
                return new Color(InvokerEMPMenu.RedItem.Value, InvokerEMPMenu.GreenItem.Value, InvokerEMPMenu.BlueItem.Value);
            }
        }

        public override bool Particle(Particle particle, string name)
        {
            if (!name.Contains("invoker_emp"))
            {
                return false;
            }

            if (!InvokerEMPMenu.EnableItem)
            {
                return true;
            }

            if (!EntityManager.GetEntities<Hero>().Any(x => !x.IsAlly(LocalHero) && !x.IsIllusion && x.HeroId == HeroId.npc_dota_hero_invoker))
            {
                return true;
            }

            var handle = particle.Handle.ToString();
            UpdateManager.BeginInvoke(1, () =>
            {
                var position = particle.GetControlPoint(0);
                DrawRange(handle, position, 675, Color, 70);

                var pos = Pos(position, InvokerEMPMenu.OnWorldItem);
                var minimapPos = MinimapPos(position, InvokerEMPMenu.OnMinimapItem);
                Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_invoker", AbilityId.invoker_emp, 0, InvokerEMPMenu.SideMessageItem, InvokerEMPMenu.SoundItem);
            });

            UpdateManager.BeginInvoke(2900, () =>
            {
                DrawRangeRemove(handle);
            });

            return true;
        }
    }
}
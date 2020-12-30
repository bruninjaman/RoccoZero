using System.Linq;
using System.Threading.Tasks;

using Divine.BeAware.Helpers;
using Divine.BeAware.MenuManager.ShowMeMore;
using Divine.BeAware.MenuManager.ShowMeMore.MoreInformation;
using Divine.SDK.Extensions;

using SharpDX;

namespace Divine.BeAware.ShowMeMore.MoreInformation
{
    internal sealed class MiranaArrow : Base
    {
        private readonly MiranaArrowMenu MiranaArrowMenu;

        private Color Color
        {
            get
            {
                return new Color(MiranaArrowMenu.RedItem.Value, MiranaArrowMenu.GreenItem.Value, MiranaArrowMenu.BlueItem.Value);
            }
        }

        public MiranaArrow(Common common)
            : base(common)
        {
            MiranaArrowMenu = MoreInformationMenu.MiranaArrowMenu;
        }

        public override bool Entity(Unit unit)
        {
            if (unit.DayVision != 500)
            {
                return false;
            }

            if (!MiranaArrowMenu.EnableItem)
            {
                return true;
            }

            var hero = EntityManager.GetEntities<Hero>().Where(x => x.Distance2D(unit) < 800).OrderBy(x => x.Distance2D(unit)).FirstOrDefault();
            if (hero != null && (hero.IsAlly(LocalHero) || hero.HeroId == HeroId.npc_dota_hero_invoker || hero.HeroId == HeroId.npc_dota_hero_mars))
            {
                return true;
            }

            var position = unit.Position;
            Arrow(unit, position, unit.Handle.ToString());

            var pos = Pos(position, MiranaArrowMenu.OnWorldItem);
            var minimapPos = MinimapPos(position, MiranaArrowMenu.OnMinimapItem);
            Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_mirana", AbilityId.mirana_arrow, 0, MiranaArrowMenu.SideMessageItem, MiranaArrowMenu.SoundItem);

            if (MiranaArrowMenu.WriteOnChatItem)
            {
                DisplayMessage();
            }

            return true;
        }

        private async void Arrow(Unit unit, Vector3 position, string id)
        {
            var color = Color;
            DrawRange($"ArrowStart_{id}", position, 200, color, 170);

            var rawGameTime = GameManager.RawGameTime;
            do
            {
                var updatePosition = unit.Position;
                var endPosition = position.Extend(updatePosition, 3000);
                if (position.ToVector2() == updatePosition.ToVector2())
                {
                    await Task.Delay(100);
                    continue;
                }

                DrawRangeRemove($"ArrowStart_{id}");
                DrawLine($"Arrow_{id}", position, endPosition, 150, 185, Color.DarkRed);

                var pos = position.Extend(endPosition, (GameManager.RawGameTime - rawGameTime) * 857);
                DrawRange($"ArrowMove_{id}", pos, 100, color, 170);
                await Task.Delay(20);
            }
            while (unit.IsValid);

            DrawRangeRemove($"ArrowStart_{id}");
            DrawRangeRemove($"ArrowMove_{id}");
            DrawLineRemove($"Arrow_{id}");
        }

        private void DisplayMessage()
        {
            switch (MenuConfig.LanguageItem)
            {
                case "EN":
                    {
                        DisplayMessage($"say_team Carefully, Mirana Arrow");
                    }
                    break;

                case "RU":
                    {
                        DisplayMessage($"say_team Осторожно, Mirana Arrow", true);
                    }
                    break;
            }
        }
    }
}
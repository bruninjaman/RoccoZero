using System.Threading.Tasks;

using Divine.BeAware.MenuManager.ShowMeMore;
using Divine.BeAware.MenuManager.ShowMeMore.MoreInformation;
using Divine.SDK.Extensions;

using SharpDX;

namespace Divine.BeAware.ShowMeMore.MoreInformation
{
    internal sealed class LinaLightStrikeArray : Base
    {
        private readonly LinaLightStrikeArrayMenu LinaLightStrikeArrayMenu;

        public LinaLightStrikeArray(Common common) : base(common)
        {
            LinaLightStrikeArrayMenu = MoreInformationMenu.LinaLightStrikeArrayMenu;
        }

        private Color Color
        {
            get
            {
                return new Color(LinaLightStrikeArrayMenu.RedItem.Value, LinaLightStrikeArrayMenu.GreenItem.Value, LinaLightStrikeArrayMenu.BlueItem.Value);
            }
        }

        public override bool Entity(Unit unit, Hero hero)
        {
            if (hero.HeroId != HeroId.npc_dota_hero_lina)
            {
                return false;
            }

            if (!LinaLightStrikeArrayMenu.EnableItem)
            {
                return true;
            }

            if (hero.IsAlly(LocalHero))
            {
                return true;
            }

            var position = unit.Position;
            LightStrikeArray(unit, position);

            if (LinaLightStrikeArrayMenu.WhenIsVisibleItem || !hero.IsVisible)
            {
                var pos = Pos(position, LinaLightStrikeArrayMenu.OnWorldItem);
                var minimapPos = MinimapPos(position, LinaLightStrikeArrayMenu.OnMinimapItem);
                Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_lina", AbilityId.lina_light_strike_array, 0, LinaLightStrikeArrayMenu.SideMessageItem, LinaLightStrikeArrayMenu.SoundItem);
            }

            return true;
        }

        private async void LightStrikeArray(Unit unit, Vector3 position)
        {
            var id = unit.Handle.ToString();
            DrawRange(id, position, 225, Color, 170);

            while (unit.IsValid)
            {
                await Task.Delay(150);
            }

            DrawRangeRemove(id);
        }
    }
}

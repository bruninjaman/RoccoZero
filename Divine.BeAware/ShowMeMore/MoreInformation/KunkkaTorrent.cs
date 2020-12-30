using System.Threading.Tasks;

using Divine.BeAware.MenuManager.ShowMeMore;
using Divine.BeAware.MenuManager.ShowMeMore.MoreInformation;
using Divine.SDK.Extensions;

using SharpDX;

namespace Divine.BeAware.ShowMeMore.MoreInformation
{
    internal sealed class KunkkaTorrent : Base
    {
        private readonly KunkkaTorrentMenu KunkkaTorrentMenu;

        public KunkkaTorrent(Common common) : base(common)
        {
            KunkkaTorrentMenu = MoreInformationMenu.KunkkaTorrentMenu;
        }

        private Color Color
        {
            get
            {
                return new Color(KunkkaTorrentMenu.RedItem.Value, KunkkaTorrentMenu.GreenItem.Value, KunkkaTorrentMenu.BlueItem.Value);
            }
        }

        public override bool Entity(Unit unit, Hero hero)
        {
            if (hero.HeroId != HeroId.npc_dota_hero_kunkka)
            {
                return false;
            }

            if (!KunkkaTorrentMenu.EnableItem)
            {
                return true;
            }

            if (hero.IsAlly(LocalHero))
            {
                return true;
            }

            var position = unit.Position;
            Torrent(unit, hero, position);

            if (KunkkaTorrentMenu.WhenIsVisibleItem || !hero.IsVisible)
            {
                var pos = Pos(position, KunkkaTorrentMenu.OnWorldItem);
                var minimapPos = MinimapPos(position, KunkkaTorrentMenu.OnMinimapItem);
                Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_kunkka", AbilityId.kunkka_torrent, 0, KunkkaTorrentMenu.SideMessageItem, KunkkaTorrentMenu.SoundItem);
            }

            return true;
        }

        private async void Torrent(Unit unit, Hero hero, Vector3 position)
        {
            var id = unit.Handle.ToString();
            DrawRange(id, position, hero.GetAbilityById(AbilityId.kunkka_torrent).GetAbilitySpecialDataWithTalent(hero, "radius"), Color, 170); //TODO

            while (unit.IsValid)
            {
                await Task.Delay(150);
            }

            DrawRangeRemove(id);
        }
    }
}

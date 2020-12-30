using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.BeAware.MenuManager.ShowMeMore.MoreInformation;
using Divine.SDK.Extensions;

using SharpDX;

namespace Divine.BeAware.ShowMeMore.MoreInformation
{
    internal sealed class WindrunnerPowershot : Base
    {
        private readonly WindrunnerPowershotMenu WindrunnerPowershotMenu;

        public WindrunnerPowershot(Common common)
            : base(common)
        {
            WindrunnerPowershotMenu = MoreInformationMenu.WindrunnerPowershotMenu;
        }

        private Color Color
        {
            get
            {
                return new Color(WindrunnerPowershotMenu.RedItem.Value, WindrunnerPowershotMenu.GreenItem.Value, WindrunnerPowershotMenu.BlueItem.Value);
            }
        }

        public override bool Particle(Particle particle, string name)
        {
            if (!name.Contains("powershot_channel"))
            {
                return false;
            }

            if (!WindrunnerPowershotMenu.EnableItem)
            {
                return true;
            }

            var hero = EntityManager.GetEntities<Hero>().FirstOrDefault(x => !x.IsAlly(LocalHero) && !x.IsIllusion && x.HeroId == HeroId.npc_dota_hero_windrunner);
            if (hero == null || !hero.IsVisible)
            {
                return true;
            }

            var startPosition = hero.Position;

            Powershot(particle, startPosition, hero);

            if (WindrunnerPowershotMenu.WhenIsVisibleItem || !hero.IsVisible)
            {
                var pos = Pos(startPosition, WindrunnerPowershotMenu.OnWorldItem);
                var minimapPos = MinimapPos(startPosition, WindrunnerPowershotMenu.OnMinimapItem);

                Verification.InfoVerification(pos, minimapPos, "npc_dota_hero_windrunner", AbilityId.windrunner_powershot, 0, WindrunnerPowershotMenu.SideMessageItem, WindrunnerPowershotMenu.SoundItem);
            }

            return true;
        }

        private async void Powershot(Particle particle, Vector3 startPosition, Hero hero)
        {
            if (!hero.IsVisible)
            {
                DrawRange("PowershotRange", startPosition, 2600, Color, 50);
            }
            else
            {
                await Task.Delay(200);

                var endPosition = hero.InFront(2600);

                DrawRange("PowershotStart", startPosition, 125, Color, 180);
                DrawRange("PowershotEnd", endPosition, 125, Color, 180);

                DrawLine("Powershot", startPosition, endPosition, 150, 210, Color);
            }

            await Task.Delay(2000);

            DrawRangeRemove("PowershotRange");
            DrawRangeRemove("PowershotStart");
            DrawRangeRemove("PowershotEnd");

            DrawLineRemove("Powershot");
        }
    }
}
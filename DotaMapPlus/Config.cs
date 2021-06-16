using Divine.Menu;
using Divine.Numerics;

namespace DotaMapPlus
{
    internal sealed class Config
    {
        private ZoomHack ZoomHack { get; }

        private ConsoleCommands ConsoleCommands { get; }

        private WeatherHack WeatherHack { get; }

        private bool Disposed { get; set; }

        public Config()
        {
            var rootMenu = MenuManager.CreateRootMenu("DotaMapPlus");
            rootMenu.SetFontColor(Color.Aqua);

            ZoomHack = new ZoomHack(rootMenu);

            ConsoleCommands = new ConsoleCommands(rootMenu);

            WeatherHack = new WeatherHack(rootMenu);
        }
    }
}
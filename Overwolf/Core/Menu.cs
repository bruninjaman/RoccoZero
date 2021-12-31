using Divine.Input;
using Divine.Menu;
using Divine.Menu.Items;

namespace Overwolf.Core
{
    internal sealed class Menu
    {

        public RootMenu RootMenu { get; }
        public MenuSwitcher OverwolfSwitcher { get; }
        public MenuToggleKey OverwolfToggleKey { get; }
        public MenuSlider OverwolfWindowSize { get; }
        public MenuSelector OverwolfBackGround { get; set; }

        public Menu()
        {
            RootMenu = MenuManager.CreateRootMenu("Overwolf");
            OverwolfSwitcher = RootMenu.CreateSwitcher("On/Off");
            OverwolfToggleKey = RootMenu.CreateToggleKey("Overwolf Toggle Key", Key.None);
        }
    }
}
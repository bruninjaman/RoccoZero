using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Divine.Menu;
using Divine.Menu.Items;

namespace ShowEnemyRoles
{
    internal sealed class Menu
    {
        public Menu()
        {
            RootMenu = MenuManager.CreateRootMenu("ShowEnemyRoles.RootMenu", "Show Enemy Roles");
            Enabled = RootMenu.CreateSwitcher("ShowEnemyRoles.Enabled", "Enabled", true);

            RootSettings = RootMenu.CreateMenu("ShowEnemyRoles.RootSettings", "Settings");
            Borders = RootSettings.CreateSwitcher("ShowEnemyRoles.Borders", "Borders", true);
            RGBMode = RootSettings.CreateSwitcher("ShowEnemyRoles.RGBMode", "RGB Mode", false);
            TextSize = RootSettings.CreateSlider("ShowEnemyRoles.TextSize", "Text Size", 15, 0, 100);
            Width = RootSettings.CreateSlider("ShowEnemyRoles.Width", "Width", 100, 0, 500);
            Height = RootSettings.CreateSlider("ShowEnemyRoles.Height", "Height", 20, 0, 100);
            Gap = RootSettings.CreateSlider("ShowEnemyRoles.Gap", "Gap", 20, 0, 200);
            RolesMovable = RootSettings.CreateSwitcher("ShowEnemyRoles.RolesMovable", "Roles Movable", false);
        }

        public RootMenu RootMenu { get; }
        public MenuSwitcher Enabled { get; }
        public Divine.Menu.Items.Menu RootSettings { get; }
        public MenuSwitcher Borders { get; }
        public MenuSwitcher RGBMode { get; }
        public MenuSlider TextSize { get; }
        public MenuSlider Width { get; }
        public MenuSlider Height { get; }
        public MenuSlider Gap { get; }
        public MenuSwitcher RolesMovable { get; }
    }
}

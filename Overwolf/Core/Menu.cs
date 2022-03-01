using Divine.Input;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Numerics;

using System.Collections.Generic;

namespace Overwolf.Core
{
    internal sealed class Menu
    {
        internal RootMenu RootMenu { get; }
        internal MenuSwitcher OverwolfSwitcher { get; }
        internal MenuToggleKey OverwolfToggleKey { get; }
        internal Divine.Menu.Items.Menu OverwolfFontColorMenu { get; }
        internal Divine.Menu.Items.Menu OverwolfCustomizationMenu { get; }
        internal MenuSlider OverwolfFontColorR { get; }
        internal MenuSlider OverwolfFontColorG { get; }
        internal MenuSlider OverwolfFontColorB { get; }
        internal MenuSlider OverwolfFontColorA { get; }
        internal MenuSwitcher OverwolfFontShadow { get; }
        internal MenuSlider OverwolfWindowSize { get; }
        internal MenuSelector OverwolfBackGround { get; set; }

        internal Menu()
        {
            RootMenu = MenuManager.CreateRootMenu("Overwolf.RootMenu", "Overwolf");
            OverwolfSwitcher = RootMenu.CreateSwitcher("Overwolf.Switcher", "On/Off");
            OverwolfToggleKey = RootMenu.CreateToggleKey("Overwolf.ToggleKey", "Overwolf Toggle Key", Key.None);
            OverwolfCustomizationMenu = RootMenu.CreateMenu("Overwolf.CustomizationMenu", "Customization");
            OverwolfFontColorMenu = OverwolfCustomizationMenu.CreateMenu("Overwolf.FontColorMenu", "Font Color");
            OverwolfFontColorR = OverwolfFontColorMenu.CreateSlider("Overwolf.R", "R", 255, 0, 255)
                .SetFontColor(Color.Red);
            OverwolfFontColorG = OverwolfFontColorMenu.CreateSlider("Overwolf.G", "G", 255, 0, 255)
                .SetFontColor(Color.Green);
            OverwolfFontColorB = OverwolfFontColorMenu.CreateSlider("Overwolf.B", "B", 255, 0, 255)
                .SetFontColor(Color.Blue);
            OverwolfFontColorA = OverwolfFontColorMenu.CreateSlider("Overwolf.A", "A", 255, 0, 255);
            OverwolfFontShadow = OverwolfCustomizationMenu.CreateSwitcher("Overwolf.FontShadow", "Font Shadow", false);
        }

        internal Dictionary<string, Color> OverwolfFontColors = new Dictionary<string, Color>()
        {
            { "Black", Color.Black },
            { "White", Color.White },
            { "Gray", Color.Gray },
        };
    }
}
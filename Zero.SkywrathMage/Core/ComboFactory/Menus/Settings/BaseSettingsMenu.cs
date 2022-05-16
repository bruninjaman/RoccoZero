using System.ComponentModel;

using Divine.Core.ComboFactory.Menus.Settings.Drawing;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Settings
{
    public class BaseSettingsMenu
    {
        [Menu("Drawing")]
        public DrawingMenu DrawingMenu { get; } = new DrawingMenu();

        [Item("Completely Disable Damage Calculation")]
        [Tooltip("Сompletely disabling Damage Calculation will give you some FPS! But you will lose KillSteal and Drawing Damage Calculation")]
        [DefaultValue(false)]
        public MenuSwitcher DisableDamageCalculationItem { get; set; }

        [Item("Completely Disable Drawing")]
        [Tooltip("Сompletely disabling Drawing will give you some FPS!")]
        [DefaultValue(false)]
        public MenuSwitcher DisableDrawingItem { get; set; }
    }
}

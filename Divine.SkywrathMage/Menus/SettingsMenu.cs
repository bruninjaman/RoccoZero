
using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class SettingsMenu
    {
        public SettingsMenu(Menu.Items.Menu menu)
        {
            var settingsMenu = menu.CreateMenu("Settings");
            DrawingMenu = new DrawingMenu(settingsMenu);

            DisableDamageCalculationItem = settingsMenu
                .CreateSwitcher("Completely Disable Damage Calculation", false)
                .SetTooltip("Сompletely disabling Damage Calculation will give you some FPS! But you will lose KillSteal and Drawing Damage Calculation");

            DisableDrawingItem = settingsMenu
                .CreateSwitcher("Completely Disable Drawing", false)
                .SetTooltip("Сompletely disabling Drawing will give you some FPS!");
        }

        public DrawingMenu DrawingMenu { get; }

        public MenuSwitcher DisableDamageCalculationItem { get; }

        public MenuSwitcher DisableDrawingItem { get; }
    }
}
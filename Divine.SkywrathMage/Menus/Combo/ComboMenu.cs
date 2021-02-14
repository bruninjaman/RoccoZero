using System.Windows.Input;

using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class ComboMenu
    {
        public ComboMenu(RootMenu rootMenu)
        {
            var comboMenu = rootMenu.CreateMenu("Combo");

            SpellsMenu = new SpellsMenu(comboMenu);
            ItemsMenu = new ItemsMenu(comboMenu);
            BlinkDaggerMenu = new BlinkDaggerMenu(comboMenu);
            AeonDiskMenu = new AeonDiskMenu(comboMenu);
            WithMuteMenu = new WithMuteMenu(comboMenu);
            MysticFlareMenu = new MysticFlareMenu(comboMenu);

            ComboHotkeyItem = comboMenu.CreateHoldKey("Combo Hotkey", Key.D);
            OrbwalkerItem = comboMenu.CreateSelector("Orbwalker:", new[] { "Default", "Distance", "Free", "Only Attack", "No Move" });
            MinDisInOrbwalkItem = comboMenu.CreateSlider("Min Distance In Orbwalk:", 600, 200, 600);
            FullDistanceModeItem = comboMenu.CreateSwitcher("Full Distance Mode", false);
            FullFreeModeItem = comboMenu.CreateSwitcher("Full Free Mode", false);
        }

        public SpellsMenu SpellsMenu { get; }

        public ItemsMenu ItemsMenu { get; }

        public BlinkDaggerMenu BlinkDaggerMenu { get; }

        public AeonDiskMenu AeonDiskMenu { get; }

        public WithMuteMenu WithMuteMenu { get; }

        public MysticFlareMenu MysticFlareMenu { get; }

        public MenuHoldKey ComboHotkeyItem { get; }

        public MenuSelector OrbwalkerItem { get; }

        public MenuSlider MinDisInOrbwalkItem { get; }

        public MenuSwitcher FullDistanceModeItem { get; }

        public MenuSwitcher FullFreeModeItem { get; }
    }
}
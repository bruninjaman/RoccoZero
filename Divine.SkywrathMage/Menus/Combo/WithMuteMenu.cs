using System.Windows.Input;

using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class WithMuteMenu
    {
        public WithMuteMenu(Menu.Items.Menu menu)
        {
            var withMuteMenu = menu.CreateMenu("With Mute");
            ToggleHotkeyItem = withMuteMenu.CreateToggleKey("Toggle Hotkey", Key.None);
            ComboHotkeyItem = withMuteMenu.CreateToggleKey("Combo Hotkey", Key.None);
        }

        public MenuToggleKey ToggleHotkeyItem { get; }

        public MenuToggleKey ComboHotkeyItem { get; }
    }
}
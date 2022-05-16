using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Combo
{
    public class BaseWithMuteMenu
    {
        [Item("Toggle Hotkey")]
        public MenuToggleKey ToggleHotkeyItem { get; set; }

        [Item("Combo Hotkey")]
        public MenuHoldKey ComboHotkey { get; set; }
    }
}

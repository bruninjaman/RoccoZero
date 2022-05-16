using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus
{
    public class BaseFarmMenu
    {
        [Item("Farm Hotkey")]
        public MenuHoldKey FarmHotkeyItem { get; set; }

        [Item("Farm:")]
        [Order(4)]
        public virtual MenuSelector FarmItem { get; set; }

        [Item("Hero Harras:")]
        [Order(5)]
        public virtual MenuSelector HeroHarrasItem { get; set; }
    }
}

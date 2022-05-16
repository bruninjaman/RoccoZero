using System.ComponentModel;

using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus.Combo
{
    public abstract class BaseComboMenu
    {
        protected BaseComboMenu()
        {
            if (BlinkDaggerMenu == null)
            {
                BlinkDaggerMenu = new BaseBlinkDaggerMenu();
            }
        }

        [Menu("Spells")]
        public abstract BaseSpellsMenu SpellsMenu { get; }

        [Menu("Items")]
        public abstract BaseItemsMenu ItemsMenu { get; }

        [Menu("Blink Dagger"), TextureDivine(@"items\item_blink.png")]
        public virtual BaseBlinkDaggerMenu BlinkDaggerMenu { get; }

        [Menu("Aeon Disk"), TextureDivine(@"items\item_aeon_disk.png")]
        public BaseAeonDiskMenu AeonDiskMenu { get; } = new BaseAeonDiskMenu();

        [Menu("With Mute")]
        public BaseWithMuteMenu WithMuteMenu { get; } = new BaseWithMuteMenu();

        [Item("Combo Hotkey")]
        public MenuHoldKey ComboHotkeyItem { get; set; }

        [Item("Orbwalker:")]
        public virtual MenuSelector OrbwalkerItem { get; set; }

        [Item("Min Distance In Orbwalk:")]
        public MenuSlider MinDisInOrbwalkItem { get; set; }

        [Item("Full Distance Mode")]
        [DefaultValue(false)]
        public MenuSwitcher FullDistanceModeItem { get; set; }

        [Item("Full Free Mode")]
        [DefaultValue(false)]
        public MenuSwitcher FullFreeModeItem { get; set; }
    }
}

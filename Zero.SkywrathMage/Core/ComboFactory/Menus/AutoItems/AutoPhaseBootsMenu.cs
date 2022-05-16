using System.ComponentModel;

using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.AutoItems
{
    public sealed class AutoPhaseBootsMenu
    {
        [Item("Enable")]
        [DefaultValue(true)]
        public MenuSwitcher EnableItem { get; set; }

        [Item("Distance Check:")]
        public MenuSlider DistanceCheckItem { get; set; }
    }
}
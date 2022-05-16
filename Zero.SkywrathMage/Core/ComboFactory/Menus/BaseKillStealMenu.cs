using System.ComponentModel;

using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus
{
    public abstract class BaseKillStealMenu
    {
        [Item("Enable")]
        [DefaultValue(true)]
        public MenuSwitcher EnableItem { get; set; }

        [Item("Disable When Combo")]
        [DefaultValue(false)]
        public MenuSwitcher DisableWhenComboItem { get; set; }

        [Item("Use:")]
        [Order(5)]
        public abstract MenuAbilityToggler AbilitiesSelection { get; set; }
    }
}

using System.ComponentModel;

using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus
{
    public class BaseDisableMenu
    {
        [Item("Enable")]
        [DefaultValue(true)]
        public MenuSwitcher EnableItem { get; set; }

        [Item("Use:")]
        [Order(2)]
        public virtual MenuAbilityToggler AbilitiesSelection { get; set; }
    }
}

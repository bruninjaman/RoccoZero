using System.ComponentModel;
using System.Text.Json.Serialization;

using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Settings.Drawing
{
    public class BaseTextPanelMenu
    {
        [Item("Combo Panel")]
        [DefaultValue(true)]
        public MenuSwitcher ComboPanelItem { get; set; }

        [Item("Move")]
        [JsonIgnore]
        [DefaultValue(false)]
        public MenuSwitcher MoveItem { get; set; }

        [Item("X")]
        public MenuSlider X { get; set; }

        [Item("Y")]
        public MenuSlider Y { get; set; }
    }
}

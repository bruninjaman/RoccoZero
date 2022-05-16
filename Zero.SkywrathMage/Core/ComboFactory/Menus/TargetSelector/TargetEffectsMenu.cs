using System.ComponentModel;

using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.TargetSelector
{
    public class TargetEffectsMenu
    {
        [Item("Enable")]
        [DefaultValue(true)]
        public MenuSwitcher EnableItem { get; set; }

        [Item("Red:")]
        public MenuSlider RedItem { get; set; }

        [Item("Green:")]
        public MenuSlider GreenItem { get; set; }

        [Item("Blue:")]
        public MenuSlider BlueItem { get; set; }

        [Item(" ")]
        public MenuText EmptyString { get; set; }

        [Item("Free Enable")]
        [DefaultValue(true)]
        public MenuSwitcher FreeEnableItem { get; set; }

        [Item("Red:")]
        public MenuSlider FreeRedItem { get; set; }

        [Item("Green:")]
        public MenuSlider FreeGreenItem { get; set; }

        [Item("Blue:")]
        public MenuSlider FreeBlueItem { get; set; }

        [Item("Effect Type:")]
        public MenuSelector EffectTypeItem { get; set; }
    }
}

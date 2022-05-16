using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Combo
{
    public class BaseBlinkDaggerMenu
    {
        [Item("Blink Activation Distance Mouse:")]
        public virtual MenuSlider BlinkActivationItem { get; set; }

        [Item("Blink Distance From Enemy:")]
        public MenuSlider BlinkDistanceEnemyItem { get; set; }
    }
}
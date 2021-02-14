
using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class BlinkDaggerMenu
    {
        public BlinkDaggerMenu(Menu.Items.Menu menu)
        {
            var blinkDaggerMenu = menu.CreateMenu("Blink Dagger").SetAbilityTexture(AbilityId.item_blink);
            BlinkActivationItem = blinkDaggerMenu.CreateSlider("Blink Activation Distance Mouse: ", 1000, 0, 1200);
            BlinkDistanceEnemyItem = blinkDaggerMenu.CreateSlider("Blink Distance From Enemy:", 300, 0, 500);
        }

        public MenuSlider BlinkActivationItem { get; }

        public MenuSlider BlinkDistanceEnemyItem { get; }
    }
}
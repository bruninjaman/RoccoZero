using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class SmartConcussiveShotMenu
    {
        public SmartConcussiveShotMenu(Menu.Items.Menu menu)
        {
            var smartConcussiveShotMenu = menu.CreateMenu("Smart Concussive Shot").SetAbilityTexture(AbilityId.skywrath_mage_concussive_shot);
            AntiFailItem = smartConcussiveShotMenu.CreateSwitcher("Anti Fail");
            UseOnlyTargetItem = smartConcussiveShotMenu.CreateSwitcher("Use Only Target").SetTooltip("This only works with Combo");
            UseInRadiusItem = smartConcussiveShotMenu.CreateSlider("Use In Radius", 1400, 800, 1600).SetTooltip("This only works with Combo");
        }

        public MenuSwitcher AntiFailItem { get; }

        public MenuSwitcher UseOnlyTargetItem { get; }

        public MenuSlider UseInRadiusItem { get; }
    }
}
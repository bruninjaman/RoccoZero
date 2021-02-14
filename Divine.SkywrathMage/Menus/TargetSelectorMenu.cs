using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class TargetSelectorMenu
    {
        public TargetSelectorMenu(Menu.Items.Menu menu)
        {
            var targetSelectorMenu = menu.CreateMenu("Target Selector");
            TargetEffectsMenu = new TargetEffectsMenu(targetSelectorMenu);

            TargetSelectorItem = targetSelectorMenu.CreateSelector("Select:", new[] { "Near Mouse", "Lowest Health" });
            TargetLockItem = targetSelectorMenu.CreateSwitcher("Target Lock");
        }

        public TargetEffectsMenu TargetEffectsMenu { get; }

        public MenuSelector TargetSelectorItem { get; }

        public MenuSwitcher TargetLockItem { get; }
    }
}
using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class BladeMailMenu
    {
        public BladeMailMenu(Menu.Items.Menu menu)
        {
            var bladeMailMenu = menu.CreateMenu("Blade Mail").SetAbilityTexture(AbilityId.item_blade_mail);
            BladeMailItem = bladeMailMenu.CreateSwitcher("Cancel Combo", false).SetTooltip("Cancel Combo if there is enemy Blade Mail");
            EulControlItem = bladeMailMenu.CreateSwitcher("Eul Control").SetTooltip("Use Eul if there is BladeMail with ULT");
        }

        public MenuSwitcher BladeMailItem { get; }

        public MenuSwitcher EulControlItem { get; }
    }
}
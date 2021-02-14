using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class AeonDiskMenu
    {
        public AeonDiskMenu(Menu.Items.Menu menu)
        {
            var aeonDiskMenu = menu.CreateMenu("Aeon Disk").SetAbilityTexture(AbilityId.item_aeon_disk);
            EnableItem = aeonDiskMenu.CreateSwitcher("Cancel Important Spells and Items").SetTooltip("If Combo Breaker is ready then it will not use Important Spells and Items");
        }

        public MenuSwitcher EnableItem { get; }
    }
}
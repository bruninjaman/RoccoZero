using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class DisableMenu
    {
        public DisableMenu(Menu.Items.Menu menu)
        {
            var disableMenu = menu.CreateMenu("Disable").SetAbilityTexture(AbilityId.item_sheepstick);
            EnableItem = disableMenu.CreateSwitcher("Enable");
            AbilitiesSelection = disableMenu.CreateAbilityToggler("Use:", new()
            {
                { AbilityId.item_sheepstick, true },
                { AbilityId.item_orchid, true },
                { AbilityId.item_bloodthorn, true },
                { AbilityId.skywrath_mage_ancient_seal, true }
            });
        }

        public MenuSwitcher EnableItem { get; }

        public MenuAbilityToggler AbilitiesSelection { get; }
    }
}
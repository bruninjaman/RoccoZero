using System.Collections.Generic;

using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class ItemsMenu
    {
        public ItemsMenu(Menu.Items.Menu menu)
        {
            var itemsMenu = menu.CreateMenu("Items");
            ItemsSelection = itemsMenu.CreateItemToggler("Use:", new Dictionary<AbilityId, bool>
            {
                { AbilityId.item_sheepstick, true },
                { AbilityId.item_orchid, true },
                { AbilityId.item_bloodthorn, true },
                { AbilityId.item_nullifier, true },
                { AbilityId.item_rod_of_atos, true },
                { AbilityId.item_ethereal_blade, true },
                { AbilityId.item_veil_of_discord, true },
                { AbilityId.item_dagon_5, true },
                { AbilityId.item_shivas_guard, true },
                { AbilityId.item_urn_of_shadows, true },
                { AbilityId.item_spirit_vessel, true },
                { AbilityId.item_blink, false }
            });
        }

        public MenuItemToggler ItemsSelection { get; }
    }
}
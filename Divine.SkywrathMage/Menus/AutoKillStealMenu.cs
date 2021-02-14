using System.Collections.Generic;

using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class AutoKillStealMenu
    {
        public AutoKillStealMenu(Menu.Items.Menu menu)
        {
            var autoKillStealMenu = menu.CreateMenu("Kill Steal");
            EnableItem = autoKillStealMenu.CreateSwitcher("Enable");
            DisableWhenComboItem = autoKillStealMenu.CreateSwitcher("Disable When Combo", false);
            AbilitiesSelection = autoKillStealMenu.CreateAbilityToggler("Use:", new Dictionary<AbilityId, bool>
            {
                { AbilityId.skywrath_mage_ancient_seal, true },
                { AbilityId.item_veil_of_discord, true },
                { AbilityId.item_ethereal_blade, true },
                { AbilityId.item_dagon_5, true },
                { AbilityId.item_shivas_guard, true },
                { AbilityId.skywrath_mage_concussive_shot, true },
                { AbilityId.skywrath_mage_arcane_bolt, true },
                { AbilityId.skywrath_mage_mystic_flare, true }
            });
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSwitcher DisableWhenComboItem { get; }

        public MenuAbilityToggler AbilitiesSelection { get; }
    }
}
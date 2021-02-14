using System.Collections.Generic;

using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class AutoComboMenu
    {
        public AutoComboMenu(Menu.Items.Menu menu)
        {
            var autoComboMenu = menu.CreateMenu("Auto Combo");
            EnableItem = autoComboMenu.CreateSwitcher("Enable");
            DisableWhenComboItem = autoComboMenu.CreateSwitcher("Disable When Combo");
            OwnerMinHealthItem = autoComboMenu.CreateSlider("Owner Min Health % To Auto Combo:", 0, 0, 70);
            SpellsSelection = autoComboMenu.CreateSpellToggler("Spells:", new Dictionary<AbilityId, bool>
            {
                { AbilityId.skywrath_mage_arcane_bolt, true },
                { AbilityId.skywrath_mage_concussive_shot, true },
                { AbilityId.skywrath_mage_ancient_seal, true },
                { AbilityId.skywrath_mage_mystic_flare, true }
            });

            ItemsSelection = autoComboMenu.CreateItemToggler("Items:", new Dictionary<AbilityId, bool>
            {
                { AbilityId.item_sheepstick, true },
                { AbilityId.item_orchid, true },
                { AbilityId.item_bloodthorn, true },
                { AbilityId.item_nullifier, true },
                { AbilityId.item_rod_of_atos, true },
                { AbilityId.item_ethereal_blade, true },
                { AbilityId.item_veil_of_discord, true },
                { AbilityId.item_dagon_5, true },
                { AbilityId.item_shivas_guard, true }
            });

            MinHealthToUltItem = autoComboMenu.CreateSlider("Target Min Health % To Ult:", 0, 0, 70);
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSwitcher DisableWhenComboItem { get; }

        public MenuSlider OwnerMinHealthItem { get; }

        public MenuSpellToggler SpellsSelection { get; }

        public MenuItemToggler ItemsSelection { get; }

        public MenuSlider MinHealthToUltItem { get; }
    }
}
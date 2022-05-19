using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class AutoComboMenu
    {
        [Item("Enable")]
        public MenuSwitcher EnableItem { get; set; }

        [Item("Disable When Combo")]
        public MenuSwitcher DisableWhenComboItem { get; set; }

        [Item("Owner Min Health % To Auto Combo:")]
        [Value(0, 0, 70)]
        public MenuSlider OwnerMinHealthItem { get; set; }

        [Item("Spells:")]
        [Value(AbilityId.skywrath_mage_arcane_bolt, true)]
        [Value(AbilityId.skywrath_mage_concussive_shot, true)]
        [Value(AbilityId.skywrath_mage_ancient_seal, true)]
        [Value(AbilityId.skywrath_mage_mystic_flare, true)]
        public MenuSpellToggler SpellsSelection { get; set; }

        [Item("Items:")]
        [Value(AbilityId.item_sheepstick, true)]
        [Value(AbilityId.item_orchid, true)]
        [Value(AbilityId.item_bloodthorn, true)]
        [Value(AbilityId.item_nullifier, true)]
        [Value(AbilityId.item_rod_of_atos, true)]
        [Value(AbilityId.item_gungir, true)]
        [Value(AbilityId.item_ethereal_blade, true)]
        [Value(AbilityId.item_veil_of_discord, true)]
        [Value(AbilityId.item_dagon_5, true)]
        [Value(AbilityId.item_shivas_guard, true)]
        public MenuItemToggler ItemsSelection { get; set; }

        [Item("Target Min Health % To Ult:")]
        [Value(0, 0, 70)]
        public MenuSlider MinHealthToUltItem { get; set; }
    }
}
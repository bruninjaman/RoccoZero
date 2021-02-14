using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class LinkenBreakerMenu
    {
        public LinkenBreakerMenu(Menu.Items.Menu menu)
        {
            var linkenBreakerMenu = menu.CreateMenu("Linken Breaker").SetAbilityTexture(AbilityId.item_sphere);
            linkenBreakerMenu.CreateText("Linkens Sphere:");

            PriorityLinkensItem = linkenBreakerMenu.CreateAbilityToggler("Priority1", "Priority:", new()
            {
                { AbilityId.item_force_staff, true },
                { AbilityId.item_cyclone, true },
                { AbilityId.item_orchid, true },
                { AbilityId.item_bloodthorn, true },
                { AbilityId.item_nullifier, true },
                { AbilityId.item_rod_of_atos, true },
                { AbilityId.item_sheepstick, true },
                { AbilityId.skywrath_mage_arcane_bolt, false },
                { AbilityId.skywrath_mage_ancient_seal, true }
            },
            true);

            linkenBreakerMenu.CreateText("LinkenBreakerMenuEmpty1", "");
            linkenBreakerMenu.CreateText("Antimage Spell Shield:");

            PrioritySpellShieldItem = linkenBreakerMenu.CreateAbilityToggler("Priority2", "Priority:", new()
            {
                { AbilityId.item_force_staff, true },
                { AbilityId.item_cyclone, true },
                { AbilityId.item_orchid, false },
                { AbilityId.item_bloodthorn, false },
                { AbilityId.item_rod_of_atos, true },
                { AbilityId.item_nullifier, false },
                { AbilityId.item_sheepstick, false },
                { AbilityId.skywrath_mage_arcane_bolt, true },
                { AbilityId.skywrath_mage_ancient_seal, false }
            },
            true);

            linkenBreakerMenu.CreateText("LinkenBreakerMenuEmpty2", "");

            UseOnlyFromRangeItem = linkenBreakerMenu.CreateSwitcher("Use Only From Range", false).SetTooltip("Use only from the Range and do not use another Ability");
        }

        public MenuAbilityToggler PriorityLinkensItem { get; }

        public MenuAbilityToggler PrioritySpellShieldItem { get; }

        public MenuSwitcher UseOnlyFromRangeItem { get; }
    }
}
using System.Collections.Generic;

using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class SpellsMenu
    {
        public SpellsMenu(Menu.Items.Menu menu)
        {
            var spellsMenu = menu.CreateMenu("Spells");
            SpellsSelection = spellsMenu.CreateSpellToggler("Use:", new()
            {
                { AbilityId.skywrath_mage_arcane_bolt, true },
                { AbilityId.skywrath_mage_concussive_shot, true },
                { AbilityId.skywrath_mage_ancient_seal, true },
                { AbilityId.skywrath_mage_mystic_flare, true }
            });
        }

        public MenuSpellToggler SpellsSelection { get; }
    }
}
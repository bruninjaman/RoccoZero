using System.Collections.Generic;

using Divine.Core.ComboFactory.Menus.Combo;

using Ensage;
using Ensage.SDK.Menu.Items;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class SpellsMenu : BaseSpellsMenu
    {
        public override ImageToggler SpellsSelection { get; set; } = new ImageToggler(new[]
        {
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_arcane_bolt.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_concussive_shot.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_ancient_seal.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_mystic_flare.ToString(), true)
        });
    }
}

using System.Collections.Generic;

using Divine.Core.ComboFactory.Menus;

using Ensage;
using Ensage.SDK.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class LinkenBreakerMenu : BaseLinkenBreakerMenu
    {
        public override PriorityChanger PriorityLinkensItem { get; set; } = new PriorityChanger(new[]
        {
            new KeyValuePair<string, bool>(AbilityId.item_force_staff.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_cyclone.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_orchid.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_bloodthorn.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_nullifier.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_rod_of_atos.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_sheepstick.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_arcane_bolt.ToString(), false),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_ancient_seal.ToString(), true)
        });

        public override PriorityChanger PrioritySpellShieldItem { get; set; } = new PriorityChanger(new[]
        {
            new KeyValuePair<string, bool>(AbilityId.item_force_staff.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_cyclone.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_orchid.ToString(), false),
            new KeyValuePair<string, bool>(AbilityId.item_bloodthorn.ToString(), false),
            new KeyValuePair<string, bool>(AbilityId.item_rod_of_atos.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_nullifier.ToString(), false),
            new KeyValuePair<string, bool>(AbilityId.item_sheepstick.ToString(), false),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_arcane_bolt.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_ancient_seal.ToString(), false)
        });
    }
}

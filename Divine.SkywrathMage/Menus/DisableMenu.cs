using System.Collections.Generic;

using Divine.Core.ComboFactory.Menus;

using Ensage;
using Ensage.SDK.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class DisableMenu : BaseDisableMenu
    {
        public override ImageToggler AbilitiesSelection { get; set; } = new ImageToggler(new[]
        {
            new KeyValuePair<string, bool>(AbilityId.item_sheepstick.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_orchid.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_bloodthorn.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_ancient_seal.ToString(), true)
        });
    }
}
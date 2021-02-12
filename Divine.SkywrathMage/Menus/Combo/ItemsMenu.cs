using System.Collections.Generic;

using Divine.Core.ComboFactory.Menus.Combo;

using Ensage;
using Ensage.SDK.Menu.Items;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class ItemsMenu : BaseItemsMenu
    {
        public override ImageToggler ItemsSelection { get; set; } = new ImageToggler(new[]
        {
            new KeyValuePair<string, bool>(AbilityId.item_sheepstick.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_orchid.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_bloodthorn.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_nullifier.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_rod_of_atos.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_ethereal_blade.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_veil_of_discord.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_dagon_5.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_shivas_guard.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_urn_of_shadows.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_spirit_vessel.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_blink.ToString(), false)
        });
    }
}

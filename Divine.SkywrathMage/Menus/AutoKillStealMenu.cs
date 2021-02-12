using System.Collections.Generic;

using Divine.Core.ComboFactory.Menus;

using Ensage;
using Ensage.SDK.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class AutoKillStealMenu : BaseKillStealMenu
    {
        public override ImageToggler AbilitiesSelection { get; set; } = new ImageToggler(new[]
        {
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_ancient_seal.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_veil_of_discord.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_ethereal_blade.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_dagon_5.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_shivas_guard.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_concussive_shot.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_arcane_bolt.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_mystic_flare.ToString(), true)
        });
    }
}
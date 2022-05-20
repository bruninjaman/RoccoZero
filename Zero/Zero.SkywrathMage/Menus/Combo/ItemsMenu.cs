using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class ItemsMenu : BaseItemsMenu
    {
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
        [Value(AbilityId.item_urn_of_shadows, true)]
        [Value(AbilityId.item_spirit_vessel, true)]
        [Value(AbilityId.item_blink, false)]
        public override MenuItemToggler ItemsSelection { get; set; }
    }
}

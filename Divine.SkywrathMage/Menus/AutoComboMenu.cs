using System.Collections.Generic;
using System.ComponentModel;

using Ensage;
using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Items;
using Ensage.SDK.Menu.ValueBinding;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class AutoComboMenu
    {
        [Item("Enable")]
        [DefaultValue(true)]
        public ValueType<bool> EnableItem { get; set; } = new ValueType<bool>();

        [Item("Disable When Combo")]
        [DefaultValue(true)]
        public bool DisableWhenComboItem { get; set; }

        [Item("Owner Min Health % To Auto Combo:")]
        public Slider<int> OwnerMinHealthItem { get; set; } = new Slider<int>(0, 0, 70);

        [Item("Spells:")]
        public ImageToggler SpellsSelection { get; set; } = new ImageToggler(new[]
        {
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_arcane_bolt.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_concussive_shot.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_ancient_seal.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.skywrath_mage_mystic_flare.ToString(), true)
        });

        [Item("Items:")]
        public ImageToggler ItemsSelection { get; set; } = new ImageToggler(new[]
        {
            new KeyValuePair<string, bool>(AbilityId.item_sheepstick.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_orchid.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_bloodthorn.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_nullifier.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_rod_of_atos.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_ethereal_blade.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_veil_of_discord.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_dagon_5.ToString(), true),
            new KeyValuePair<string, bool>(AbilityId.item_shivas_guard.ToString(), true)
        });

        [Item("Target Min Health % To Ult:")]
        public Slider<int> MinHealthToUltItem { get; set; } = new Slider<int>(0, 0, 70);
    }
}
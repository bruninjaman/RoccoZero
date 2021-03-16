using System.Collections.Generic;
using System.Linq;

using Divine;

using Ensage;
using Ensage.Common.Menu;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Inventory.Metadata;
using TinkerCrappahilationPaid.Abilities;

namespace TinkerCrappahilationPaid
{
    public class AbilitiesInCombo
    {
        public AbilitiesInCombo(TinkerCrappahilationPaid main)
        {
            main.Context.Inventory.Attach(this);
            var dict = new Dictionary<string, bool>
            {
                {AbilityId.item_blink.ToString(), true},
                {AbilityId.item_soul_ring.ToString(), true},
                {AbilityId.item_veil_of_discord.ToString(), true},
                {AbilityId.item_ethereal_blade.ToString(), true},
                {AbilityId.item_dagon.ToString(), true},
                {AbilityId.item_dagon_2.ToString(), true},
                {AbilityId.item_dagon_3.ToString(), true},
                {AbilityId.item_dagon_4.ToString(), true},
                {AbilityId.item_dagon_5.ToString(), true},
                {AbilityId.tinker_heat_seeking_missile.ToString(), true},
                {AbilityId.item_sheepstick.ToString(), true},
                {AbilityId.item_ghost.ToString(), true},
                {AbilityId.item_black_king_bar.ToString(), true},
                {AbilityId.item_shivas_guard.ToString(), true},
                {AbilityId.tinker_laser.ToString(), true},
                {AbilityId.item_glimmer_cape.ToString(), true},
                {AbilityId.tinker_rearm.ToString(), true},
            };
            var newMenu = main.Config.Factory.Menu("Abilities");
            main.Config.ItemsInCombo = newMenu.Item("ItemsInCombo", new AbilityToggler(dict));
            var list = dict.Keys.ToList();
            main.Config.Priority = newMenu.Item("Priority", new PriorityChanger(list));

            dict = new Dictionary<string, bool>
            {
                {AbilityId.item_blink.ToString(), true},
                {AbilityId.item_soul_ring.ToString(), true},
                {AbilityId.item_bottle.ToString(), true},
                {AbilityId.item_ghost.ToString(), true},
                {AbilityId.item_shivas_guard.ToString(), true},
                {AbilityId.item_glimmer_cape.ToString(), true},
            };

            main.Config.ItemsInSpamCombo = newMenu.Item("Spam Combo", new AbilityToggler(dict));

            Laser = new Laser(main.Me.Spellbook.Spell1);
            Rocket = new Rockets(main.Me.Spellbook.Spell2);
            Rearm = new Rearm(main.Me.Spellbook.Spell4);
            March = main.Me.Spellbook.Spell3;
        }

        [ItemBinding]
        public item_blink Blink { get; set; }
        [ItemBinding]
        public item_blink Hex { get; set; }
        [ItemBinding]
        public item_dagon Dagon { get; set; }
        [ItemBinding]
        public item_dagon_2 Dagon2 { get; set; }
        [ItemBinding]
        public item_dagon_3 Dagon3 { get; set; }
        [ItemBinding]
        public item_dagon_4 Dagon4 { get; set; }
        [ItemBinding]
        public item_dagon_5 Dagon5 { get; set; }
        [ItemBinding]
        public item_ghost Ghost { get; set; }
        [ItemBinding]
        public item_veil_of_discord Veil { get; set; }
        [ItemBinding]
        public item_soul_ring SoulRing { get; set; }
        [ItemBinding]
        public item_ethereal_blade EtherealBlade { get; set; }
        [ItemBinding]
        public item_shivas_guard Shiva { get; set; }
        [ItemBinding]
        public item_black_king_bar Bkb { get; set; }
        [ItemBinding]
        public item_travel_boots Travel { get; set; }
        [ItemBinding]
        public item_travel_boots_2 Travel2 { get; set; }
        [ItemBinding]
        public item_bottle Bottle { get; set; }
        [ItemBinding]
        public item_glimmer_cape Glimmer { get; set; }

        public Laser Laser { get; }
        public Rockets Rocket { get; }
        public Rearm Rearm { get; }
        public Ability March { get; }
    }
}
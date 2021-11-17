using Divine.Entity.Entities.Abilities.Components;

namespace TinkerEW
{
    internal static class Data
    {
        internal static class Menu
        {
            public static readonly Dictionary<AbilityId, bool> ComboItems = new Dictionary<AbilityId, bool>()
            {
                { AbilityId.item_soul_ring, true },
                { AbilityId.item_ghost, true },
                { AbilityId.item_eternal_shroud, true },
                { AbilityId.item_veil_of_discord, true },
                { AbilityId.item_blink, true },
                { AbilityId.item_dagon, true },
                { AbilityId.item_ethereal_blade, true },
                { AbilityId.item_shivas_guard, true },
                { AbilityId.item_sheepstick, true },
                { AbilityId.item_orchid, true },
                { AbilityId.item_bloodthorn, true },
                { AbilityId.item_rod_of_atos, true },
                { AbilityId.item_glimmer_cape, true },
                { AbilityId.item_guardian_greaves, true },
                { AbilityId.item_lotus_orb, true },
                { AbilityId.item_nullifier, true }
            };

            public static readonly Dictionary<AbilityId, bool> ComboAbilities = new Dictionary<AbilityId, bool>()
            {
                { AbilityId.tinker_laser, true },
                { AbilityId.tinker_heat_seeking_missile, true },
                { AbilityId.tinker_march_of_the_machines, true },
                { AbilityId.tinker_defense_matrix, true },
                { AbilityId.tinker_rearm, true },
            };

            public static readonly string[] TargetSelectorModes = new string[]
            {
                "Nearest To Cursor",
                "In Radius From Cursor"
            };

            public static readonly string[] ComboBlinkModes = new string[]
            {
                "1`st in radius then to cursor",
                "In radius",
                "To cursor"
            };

            public static readonly string[] LinkenBreakerModes = new string[]
            {
                "Cheapest",
                "First what can be used (not Hex)",
                "Laser"
            };
        }

        internal static class TargetSelector
        {
            public enum TargetSelectorModes
            {
                NearestToCursor = 0,
                InRadiusFromCursor = 1
            }
        }
    }
}

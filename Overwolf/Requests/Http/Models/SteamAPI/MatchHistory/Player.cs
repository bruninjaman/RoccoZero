using System.Text.Json.Serialization;

namespace Overwolf.Requests.Http.Models.SteamAPI.MatchHistory
{
    public sealed class Player
    {
        [JsonPropertyName("account_id")]
        public long AccountId { get; set; }

        [JsonPropertyName("player_slot")]
        public int PlayerSlot { get; set; }

        [JsonPropertyName("hero_id")]
        public int HeroId { get; set; }
    }
}
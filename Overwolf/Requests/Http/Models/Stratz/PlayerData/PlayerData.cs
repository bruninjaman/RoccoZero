using System.Text.Json.Serialization;

namespace Overwolf.Requests.Http.Models.PlayerData
{
    internal sealed class PlayerData
    {
        [JsonPropertyName("steamAccountId")]
        public int SteamAccounId { get; set; }

        [JsonPropertyName("steamAccount")]
        public SteamAccount SteamAccount { get; set; }
    }
}
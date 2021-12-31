using System.Text.Json.Serialization;

namespace Overwolf.Requests.Http.Models.PlayerData
{
    internal sealed class SteamAccount
    {
        [JsonPropertyName("steamId")]
        public int SteamId { get; set; }

        [JsonPropertyName("isAnonymous")]
        public bool IsAnonymous { get; set; }

    }
}
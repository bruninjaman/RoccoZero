
using System.Text.Json.Serialization;

namespace Overwolf.Requests.Http.Models.SteamAPI.MatchHistory
{
    internal sealed class MatchHistory
    {
        [JsonPropertyName("result")]
        public Result Result { get; set; }
    }
}

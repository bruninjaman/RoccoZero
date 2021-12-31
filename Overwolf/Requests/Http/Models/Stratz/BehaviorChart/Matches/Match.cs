using System.Text.Json.Serialization;

namespace Overwolf.Requests.Http.Models.BehaviorChart.Matches
{
    internal sealed class Match
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("didRadiantWin")]
        public bool DidRadiantWin { get; set; }

        [JsonPropertyName("endDateTime")]
        public int EndDateTime { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("player")]
        public Player Player { get; set; }

    }
}

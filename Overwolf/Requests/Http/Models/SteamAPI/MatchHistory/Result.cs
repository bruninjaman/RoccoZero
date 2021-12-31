using System.Text.Json.Serialization;

namespace Overwolf.Requests.Http.Models.SteamAPI.MatchHistory
{
    public sealed class Result
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("num_results")]
        public int NumResults { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }

        [JsonPropertyName("results_remaining")]
        public int ResultsRemaining { get; set; }

        [JsonPropertyName("matches")]
        public Match[] Matches { get; set; }

    }
}
using Overwolf.Requests.Http.Models.BehaviorChart.Heroes;
using Overwolf.Requests.Http.Models.BehaviorChart.Matches;

using System.Text.Json.Serialization;

namespace Overwolf.Requests.Http.Models.BehaviorChart
{
    internal sealed class BehaviorChart
    {
        [JsonPropertyName("matchCount")]
        public int MatchCount { get; set; }

        [JsonPropertyName("winCount")]
        public int WinCount { get; set; }

        [JsonPropertyName("heroes")]
        public Hero[] Heroes { get; set; }

        [JsonPropertyName("matches")]
        public Match[] Matches { get; set; }
    }
}

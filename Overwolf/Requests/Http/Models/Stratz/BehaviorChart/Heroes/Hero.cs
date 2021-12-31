using Divine.Entity.Entities.Units.Heroes.Components;

using System.Text.Json.Serialization;

namespace Overwolf.Requests.Http.Models.BehaviorChart.Heroes
{
    internal sealed class Hero
    {

        [JsonPropertyName("heroId")]
        public HeroId HeroId { get; set; }

        [JsonPropertyName("matchCount")]
        public int MatchCount { get; set; }

        [JsonPropertyName("winCount")]
        public int WinCount { get; set; }

        [JsonPropertyName("avgImp")]
        public int AvgImp { get; set; }

        [JsonPropertyName("mvpCount")]
        public int MvpCount { get; set; }

        [JsonPropertyName("topCoreCount")]
        public int TopCoreCount { get; set; }

        [JsonPropertyName("topSupportCount")]
        public int TopSupportCount { get; set; }

    }
}

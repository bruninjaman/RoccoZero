using System.Text.Json.Serialization;

namespace Overwolf.Requests.Http.Models.BehaviorChart.Matches
{
    internal sealed class Player
    {

        [JsonPropertyName("playerSlot")]
        public int PlayerSlot { get; set; }

        [JsonPropertyName("heroId")]
        public int HeroId { get; set; }

        [JsonPropertyName("award")]
        public int Award { get; set; }

        [JsonPropertyName("isVictory")]
        public int IsVictory { get; set; }

        [JsonPropertyName("imp")]
        public int Imp { get; set; }

    }
}

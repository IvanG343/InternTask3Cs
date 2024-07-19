using System.Text.Json.Serialization;

namespace SteamTopSellers.Models
{
    // Models for Game Info JSON Deserialization
    public class GameInfoRoot
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public GameInfoData Data { get; set; }
    }

    public class GameInfoData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("is_free")]
        public bool IsFree { get; set; }

        [JsonPropertyName("price_overview")]
        public GamePriceOverview PriceOverview { get; set; }
    }

    public class GamePriceOverview
    {
        [JsonPropertyName("final_formatted")]
        public string FinalFormatted { get; set; }
    }
}

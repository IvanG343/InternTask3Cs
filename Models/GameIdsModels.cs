using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SteamTopSellers.Models
{
    // Models for Game List JSON Deserialization
    public class GameIdsRoot
    {
        [JsonPropertyName("response")]
        public GameIdsResponse Response { get; set; }
    }

    public class GameIdsResponse
    {
        [JsonPropertyName("ids")]
        public List<GameId> Ids { get; set; }
    }

    public class GameId
    {
        [JsonPropertyName("appid")]
        public int? AppId { get; set; }
    }
}

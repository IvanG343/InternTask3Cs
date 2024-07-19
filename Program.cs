using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamTopSellers
{
    internal class Program
    {
        public const string GET_GAMES_LIST_URL = "https://api.steampowered.com/IStoreQueryService/Query/v1/";
        public const string GET_GAME_INFO_URL = "https://store.steampowered.com/api/appdetails?appids=";

        public static int rowsToDisplay = 10;

        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            string gamesListQueryParams = "{\"query\":{\"count\":\"15\",\"sort\":\"10\"},\"context\":{\"country_code\":\"RU\"}}";
            string gamesListQuery = $"{GET_GAMES_LIST_URL}?input_json={Uri.EscapeDataString(gamesListQueryParams)}";

            List<int> gameIds = await GetTopSellingsList(gamesListQuery);

            for (int i = 0; i < rowsToDisplay; i++)
            {
                int gameId = gameIds[i];

                GameInfo gameInfo = await GetGameInfo(GET_GAME_INFO_URL, gameId);
                Console.WriteLine(i + 1);
                Console.WriteLine(gameInfo.Name);
                Console.WriteLine(gameInfo.FormattedPrice);
            }

            Console.ReadKey();
        }

        static async Task<List<int>> GetTopSellingsList(string url)
        {
            string responseBody = await GetResponseContent(url);
            var responseObject = JsonSerializer.Deserialize<GameIdsRoot>(responseBody);

            List<int> result = new List<int>();

            if(responseObject.Response.Ids != null)
            {
                foreach (var id in responseObject.Response.Ids)
                    if(id.AppId.HasValue)
                        result.Add(id.AppId.Value);
            }

            return result;
        }

        static async Task<GameInfo> GetGameInfo(string url, int gameId)
        {
            string gameInfoQuery = $"{url}{gameId}&cc=RU";
            string responseBody = await GetResponseContent(gameInfoQuery);

            var gameInfoResponse = JsonSerializer.Deserialize<Dictionary<string, GameInfoRoot>>(responseBody);

            if(gameInfoResponse[gameId.ToString()].Data != null)
            {
                return new GameInfo
                {
                    Name = gameInfoResponse[gameId.ToString()].Data.Name,
                    FormattedPrice = gameInfoResponse[gameId.ToString()].Data.IsFree ? "Free to Play" : gameInfoResponse[gameId.ToString()].Data.PriceOverview.FinalFormatted,
                };
            }

            return null;

        }

        static async Task<string> GetResponseContent(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        //GamesList
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

        //GameInfo
        public class GameInfoRoot
        {
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

        public class GameInfo
        {
            public string Name { get; set; }
            public string FormattedPrice { get; set; }
        }
    }
}

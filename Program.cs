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

        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            string gamesListQueryParams = "{\"query\":{\"count\":\"15\",\"sort\":\"10\"},\"context\":{\"country_code\":\"RU\"}}";
            string gamesListQuery = $"{GET_GAMES_LIST_URL}?input_json={Uri.EscapeDataString(gamesListQueryParams)}";

            List<int> gameIds = await GetGamesIds(gamesListQuery);

            foreach (var id in gameIds)
            {
                Console.WriteLine(id);
            }

            Console.ReadKey();
        }

        static async Task<List<int>> GetGamesIds(string url)
        {
            //GetResponseContent
            string responseBody = await GetResponseContent(url);
            //Parse JSON with TopSellers IDs
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


        //static async Task<string> GetGameInfo()
        //GetResponseContent
        //Deserialize JSON with Game Info by appId

        static async Task<string> GetResponseContent(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

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
}

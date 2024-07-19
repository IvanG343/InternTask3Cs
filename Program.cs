using System;
using System.Net.Http;

namespace SteamTopSellers
{
    internal class Program
    {
        public const string GET_GAMES_LIST_URL = "https://api.steampowered.com/IStoreQueryService/Query/v1/";
        public const string GET_GAME_INFO_URL = "https://store.steampowered.com/api/appdetails?appids=";

        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            string gamesListQueryParams = "{\"query\":{\"count\":\"15\",\"sort\":\"10\"},\"context\":{\"country_code\":\"RU\"}}";
            string gamesListQuery = $"{GET_GAMES_LIST_URL}?input_json={Uri.EscapeDataString(gamesListQueryParams)}";

            //List<int> gameIds = GetGamesIds(gamesListQuery);
        }

        //static async Task<string> GetGamesIds(string url)
        //GetResponseContent
        //Deserialize JSON with TopSellers IDs

        //static async Task<string> GetGameInfo()
        //GetResponseContent
        //Deserialize JSON with Game Info by appId

        //static async Task<string> GetResponseContent()
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SteamTopSellers.Models;

namespace SteamTopSellers
{
    internal class Program
    {
        private const string GetGamesListUrl = "https://api.steampowered.com/IStoreQueryService/Query/v1/";
        private const string GetGameInfoUrl = "https://store.steampowered.com/api/appdetails?appids=";
        private const int GamesToDisplay = 10;

        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            try
            {
                List<int> gameIds = await GetTopSellingsListAsync();


                Console.WriteLine("10 самых продаваемых игр на текущий момент по объёму выручки Steam (РФ)\n");
                for (int i = 0; i < GamesToDisplay; i++)
                {
                    if(i >= gameIds.Count)
                    {
                        Console.WriteLine($"{i + 1}. ....");
                        continue;
                    }

                    int gameId = gameIds[i];

                    GameDetails gameDetails = await GetGameInfoAsync(gameId);

                    Console.WriteLine($"{i + 1}. {gameDetails.Name}");
                    Console.WriteLine(gameDetails.FormattedPrice);
                    Console.WriteLine();
                }
            } 
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON error: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e.Message}");
            }

            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

        static async Task<List<int>> GetTopSellingsListAsync()
        {
            string gamesListQueryParams = "{\"query\":{\"count\":\"15\",\"sort\":\"10\"},\"context\":{\"country_code\":\"RU\"}}";
            string gamesListQuery = $"{GetGamesListUrl}?input_json={Uri.EscapeDataString(gamesListQueryParams)}";

            string responseBody = await GetResponseContentAsync(gamesListQuery);
            var responseObject = JsonSerializer.Deserialize<GameIdsRoot>(responseBody);

            List<int> result = new List<int>();

            if(responseObject?.Response?.Ids != null)
            {
                foreach (var id in responseObject.Response.Ids)
                    if(id.AppId.HasValue)
                        result.Add(id.AppId.Value);
            }

            return result;
        }

        static async Task<GameDetails> GetGameInfoAsync(int gameId)
        {
            string gameInfoQuery = $"{GetGameInfoUrl}{gameId}&cc=RU";
            string responseBody = await GetResponseContentAsync(gameInfoQuery);

            var gameInfoResponse = JsonSerializer.Deserialize<Dictionary<string, GameInfoRoot>>(responseBody);

            if (gameInfoResponse.TryGetValue(gameId.ToString(), out var gameInfo) && gameInfo.Success)
            {
                var data = gameInfo.Data;
                return new GameDetails
                {
                    Name = data.Name,
                    FormattedPrice = data.IsFree ? "Free to Play" : data.PriceOverview.FinalFormatted,
                };
            }
            else
            {
                throw new Exception("Error retrieving game data");
            }
        }

        static async Task<string> GetResponseContentAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

            if(!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{response.StatusCode}");
            }

            return responseBody;
        }

    }
}

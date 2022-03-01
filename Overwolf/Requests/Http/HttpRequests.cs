using Divine.Game;
using Divine.Service;
using Divine.Zero.Log;
using Divine.Zero.Pipes.Formatters;

using Overwolf.Exstensions;
using Overwolf.Requests.Http.Models.Divine;
using Overwolf.Requests.Http.Models.Divine.GraphQL;
using Overwolf.Requests.Http.Models.SteamAPI;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Overwolf.Requests.Http
{
    internal static class HttpRequests
    {
        private static readonly HttpClient httpClient;

        static HttpRequests()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "stratz-user-agent");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", DivineService.AccessToken);
        }

        internal static async Task<MatchDetails> GetDivineMatchDetailsAsync(ulong matchId)
        {
            var swatch = new Stopwatch();
            swatch.Start();
            var task = await httpClient.GetAsync(new Uri($"https://stratz.divinecheat.com/Steam/{matchId}"))
                .ConfigureAwait(false);
            swatch.Stop();

            LogManager.Debug($"GetDivineMatchDetails for {matchId} StatusCode: {(int)task.StatusCode} {task.StatusCode} was {(int)(swatch.Elapsed.TotalMilliseconds)}ms");

            if (task.IsSuccessStatusCode)
            {
                return await task.Content.ReadFromJsonAsync<MatchDetails>().ConfigureAwait(false);
            }
            return null;
        }

        internal static async Task<StratzData> GetDivineStratzDataAsync(uint accountId, DateTime startDateTime)
        {
            var data = @$"{{
                ""mainInfoArgs"": {{ }},
                ""heroPerformance"": {{
                    ""startDateTime"": ""{((DateTimeOffset)startDateTime).ToUnixTimeSeconds()}""
                }},
                ""summaryArgs"": {{ }}
            }}";

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var swatch = new Stopwatch();
            swatch.Start();
            var task = await httpClient.PostAsync(new Uri($"https://stratz.divinecheat.com/Stratz/{accountId}"), content)
                .ConfigureAwait(false);
            swatch.Stop();
            LogManager.Debug($"GetDivineStratzData for {accountId} StatusCode: {(int)task.StatusCode} {task.StatusCode} was {(int)(swatch.Elapsed.TotalMilliseconds)}ms");

            if (task.IsSuccessStatusCode)
            {
                return await task.Content.ReadFromJsonAsync<StratzData>().ConfigureAwait(false);
            }

            return null;
        }

        internal static async Task<StratzGraphQLData> GetDivineStratzGraphQLDataAsync(List<string> players, DateTime startDateTime)
        {
            var data = @$"{{
                ""players"": {JsonSerializer.Serialize(players)},
                ""startDateTime"": ""{((DateTimeOffset)startDateTime).ToUnixTimeSeconds()}""
            }}";

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var swatch = new Stopwatch();
            swatch.Start();
            var task = await httpClient.PostAsync(new Uri($"https://stratz.divinecheat.com/Stratz/graphQL"), content)
                .ConfigureAwait(false);
            swatch.Stop();
            LogManager.Debug($"GetDivineStratzGraphQLData StatusCode: {(int)task.StatusCode} {task.StatusCode} was {(int)(swatch.Elapsed.TotalMilliseconds)}ms");

            if (task.IsSuccessStatusCode)
            {
                //var str = await task.Content.ReadAsStringAsync();
                //Console.WriteLine(str);
                return await task.Content.ReadFromJsonAsync<StratzGraphQLData>().ConfigureAwait(false);
            }

            return null;
        }
    }
}
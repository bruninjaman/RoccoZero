using Divine.Zero.Log;

using Overwolf.Requests.Http.Models.Divine;
using Overwolf.Requests.Http.Models.SteamAPI;
using Overwolf.Requests.Http.Models.SteamAPI.MatchHistory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;

using static Overwolf.Data.Data;

namespace Overwolf.Requests.Http
{
    internal static class HttpRequests
    {
        private const string apiKey = "66EEABA8058D31C45B5116E707ED3C9E";
        private static readonly HttpClient httpClient = new() { };
        public static async Task<MatchHistory> GetMatchHistoryAsync(int accountId)
        {
            var task = await httpClient.GetAsync(new Uri($"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/V001/?key={apiKey}&account_id={accountId}&matches_requested=100"))
                .ConfigureAwait(false);

            if (task.IsSuccessStatusCode)
            {
                return await task.Content.ReadFromJsonAsync<MatchHistory>().ConfigureAwait(false);
            }
            return null;
        }

        public static async Task<HttpResponseMessage> GetMatchDetailsAsync(ulong matchId)
        {
            var task = await httpClient.GetAsync(new Uri($"https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/v1/?key={apiKey}&match_id={matchId}"))
                .ConfigureAwait(false);
            LogManager.Debug($"GetMatchDetails for {matchId} StatusCode: {(int)task.StatusCode} {task.StatusCode}");

            return task;
        }

        public static async Task<StratzData> GetDivineStratzDataAsync(uint accountId, DateTime startDateTime)
        {
            var data = @$"{{
                ""mainInfoArgs"": {{ }},
                ""heroPerformance"": {{
                    ""startDateTime"": ""{((DateTimeOffset)startDateTime).ToUnixTimeSeconds()}""
                }},
                ""summaryArgs"": {{ }}
            }}";
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "stratz-user-agent");

            var task = await httpClient.PostAsync(new Uri($"https://stratz.divinecheat.com/Stratz/{accountId}"), content)
                .ConfigureAwait(false);
            LogManager.Debug($"GetDivineStratzData for {accountId} StatusCode: {(int)task.StatusCode} {task.StatusCode}");

            if (task.IsSuccessStatusCode)
            {
                return await task.Content.ReadFromJsonAsync<StratzData>().ConfigureAwait(false);
            }

            return null;
        }
    }
}

using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Network;
using Divine.Network.EventArgs;
using Divine.Network.GC;
using Divine.Network.GCSO;
using Divine.Update;

using Overwolf.Exstensions;
using Overwolf.Helpers;
using Overwolf.Requests.Http;
using Overwolf.Requests.Http.Models.SteamAPI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using static Overwolf.Data.Data;

namespace Overwolf.Core
{
    internal sealed class CoreMain
    {
        public Dictionary<int, PlayerData> playerTable = new Dictionary<int, PlayerData>();
        private List<HttpMessage> httpRequests = new List<HttpMessage>();
        private List<ProtoMessage> profileRequests = new List<ProtoMessage>();
        private List<ProtoMessage> matchDetailsRequests = new List<ProtoMessage>();
        private uint profileRequestsSent = 0;
        private uint matchDetailsRequestsSent = 0;
        private Sleeper profileRequestsSleeper = new Sleeper();
        private Sleeper matchDetailsRequestsSleeper = new Sleeper();
        private List<TempClass> TempTable = new List<TempClass>()
        {
            new TempClass { id = 120303454, partyId = 123123, heroId = 12, name = "ks", laneSelectonFlags = LaneSelectonFlags.hardsupport},
            new TempClass { id = 352031777, partyId = 123123, heroId = 17, name = "Angel ", laneSelectonFlags = LaneSelectonFlags.safelane},
            new TempClass { id = 452400903, partyId = 454545, heroId = 26, name = "Greedy", laneSelectonFlags = LaneSelectonFlags.softsupport},
            new TempClass { id = 493967542, partyId = 454545, heroId = 50, name = "Hitman", laneSelectonFlags = LaneSelectonFlags.midlane},
            new TempClass { id = 924944185, partyId = 454545, heroId = 76, name = "Davai Lama", laneSelectonFlags = LaneSelectonFlags.offlane},
            new TempClass { id = 103656457, partyId = 999898, heroId = 83, name = "tw.tv/SpeedManq", laneSelectonFlags = LaneSelectonFlags.offlane},
            new TempClass { id = 117311875, partyId = 999898, heroId = 31, name = "Pablo", laneSelectonFlags = LaneSelectonFlags.softsupport},
            new TempClass { id = 171498131, partyId = 345345, heroId = 49, name = "Save-", laneSelectonFlags = LaneSelectonFlags.hardsupport},
            new TempClass { id = 216264942, partyId = 345345, heroId = 22, name = "monkemajor", laneSelectonFlags = LaneSelectonFlags.midlane},
            new TempClass { id = 218172539, partyId = 345345, heroId = 66, name = "Danial", laneSelectonFlags = LaneSelectonFlags.safelane},
        };

        public CoreMain(Context context)
        {
            UpdateManager.Update += UpdateManager_Update;
            NetworkManager.GCSOMessageUpdate += NetworkManager_GCSOMessageUpdate;
            NetworkManager.GCMessageReceived += NetworkManager_GCMessageReceived;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            for (int i = 0; i < 10; i++)
            {
                if (!playerTable.ContainsKey(i))
                    playerTable.Add(i, new PlayerData());

                playerTable[i].id = TempTable[i].id;
                playerTable[i].partyId = TempTable[i].partyId;
                playerTable[i].heroId = (HeroId)TempTable[i].heroId;
                playerTable[i].name = TempTable[i].name;
                playerTable[i].laneSelectonFlags = TempTable[i].laneSelectonFlags;

                profileRequests.Add(new ProtoMessage
                {
                    msgId = GCMessageId.CMsgProfileRequest,
                    accountId = playerTable[i].id,
                    jobId = (ulong)(i + 1)
                });
                httpRequests.Add(new HttpMessage
                {
                    requestType = RequestType.GetDivineStratzData,
                    accountId = playerTable[i].id,
                    playerNumber = i,
                    startDateTime = DateTime.UtcNow.AddMonths(-1)
                });
            }
        }

        public class TempClass
        {
            public uint id;
            public ulong partyId;
            public int heroId;
            public string name;
            public LaneSelectonFlags laneSelectonFlags;
        }

        private void NetworkManager_GCMessageReceived(GCMessageReceivedEventArgs e)
        {
            if (e.Protobuf.MessageId != GCMessageId.CMsgProfileResponse
                && e.Protobuf.MessageId != GCMessageId.CMsgGCMatchDetailsResponse
                && e.Protobuf.MessageId != GCMessageId.CMsgClientToGCRequestPlayerRecentAccomplishmentsResponse
                && e.Protobuf.MessageId != GCMessageId.CMsgClientToGCRequestPlayerHeroRecentAccomplishmentsResponse)
            {
                return;
            }
            Console.WriteLine("===================================================================");
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} | {e.Protobuf.MessageId} JobId: {e.JobId} Result: {e.Protobuf.ToJson()["result"]}");
            Console.WriteLine("===================================================================");
            //Console.WriteLine(e.Protobuf.ToJson());

            var jobId = (int)(e.JobId - 1);

            //System.Console.WriteLine(e.Protobuf.MessageId);
            //System.Console.WriteLine("1: " + e.JobId);
            //System.Console.WriteLine("2: " + jobId % 10);
            //System.Console.WriteLine("3: " + jobId / 10);
            var playerId = jobId % 10;
            if (!playerTable.ContainsKey(playerId)) return;
            var player = playerTable[playerId];

            switch (e.Protobuf.MessageId)
            {
                case GCMessageId.CMsgProfileResponse:
                    {
                        if (e.Protobuf.ToJson()["result"].GetValue<string>() == "k_eTooBusy")
                        {
                            profileRequests.Add(new ProtoMessage
                            {
                                msgId = GCMessageId.CMsgProfileRequest,
                                accountId = player.id,
                                jobId = e.JobId
                            });
                            profileRequestsSleeper.Sleep(1001);
                            return;
                        }
                        else if (e.Protobuf.ToJson()["result"].GetValue<string>() != "k_eSuccess")
                        {
                            return;
                        }

                        var recentMatches = e.Protobuf.ToJson()["recent_matches"].AsArray();
                        foreach (var match in recentMatches)
                        {
                            var index = recentMatches.IndexOf(match);
                            if (!player.recentMatches.ContainsKey(index))
                                player.recentMatches.Add(index, new Match());

                            if (ulong.TryParse(match["match_id"].GetValue<string>(), out var match_id))
                                player.recentMatches[index].matchId = match_id;

                            //System.Console.WriteLine("SendGCMatchDetailsRequest: " + ((ulong)(index * 10) + (ulong)(playerId + 1)));
                            //NetworkManager.SendGCMatchDetailsRequest(match_id, (ulong)(index * 10) + (ulong)(playerId + 1));

                            httpRequests.Add(new HttpMessage
                            {
                                requestType = RequestType.GetMatchDetails,
                                matchId = match_id,
                                playerNumber = playerId,
                                matchNumber = index,
                            });

                            //matchDetailsRequests.Add(new ProtoMessage
                            //{
                            //    msgId = GCMessageId.CMsgGCMatchDetailsRequest,
                            //    matchId = match_id,
                            //    jobId = (ulong)(index * 10) + (ulong)(playerId + 1)
                            //});

                            player.recentMatches[index].matchTimestamp = match["match_timestamp"].GetValue<uint>();
                            player.recentMatches[index].performanceRating = match["performance_rating"].GetValue<int>();
                            player.recentMatches[index].heroId = (HeroId)match["hero_id"].GetValue<uint>();
                            player.recentMatches[index].wonMatch = match["won_match"].GetValue<bool>();
                        }
                        break;
                    }
                case GCMessageId.CMsgGCMatchDetailsResponse:
                    {
                        var matchNumber = jobId / 10;
                        if (e.Protobuf.ToJson()["result"].GetValue<int>() == 2)
                        {
                            matchDetailsRequests.Add(new ProtoMessage
                            {
                                msgId = GCMessageId.CMsgGCMatchDetailsRequest,
                                matchId = (uint)player.recentMatches[matchNumber].matchId,
                                jobId = e.JobId
                            });
                            matchDetailsRequestsSleeper.Sleep(1001);
                            return;
                        }
                        else if (e.Protobuf.ToJson()["result"].GetValue<int>() != 1)
                        {
                            return;
                        }
                        var matchDetails = e.Protobuf.ToJson();
                        var match = matchDetails["match"];
                        player.recentMatches[matchNumber].duration = match["duration"].GetValue<uint>();

                        foreach (var playerr in match["players"].AsArray())
                        {
                            if (player.id == playerr["account_id"].GetValue<uint>())
                            {
                                player.recentMatches[matchNumber].kills = playerr["kills"].GetValue<uint>();
                                player.recentMatches[matchNumber].deaths = playerr["deaths"].GetValue<uint>();
                                player.recentMatches[matchNumber].assists = playerr["assists"].GetValue<uint>();
                                player.recentMatches[matchNumber].XPPerMin = playerr["XP_per_min"].GetValue<uint>();
                                player.recentMatches[matchNumber].goldPerMin = playerr["gold_per_min"].GetValue<uint>();
                                player.recentMatches[matchNumber].laneSelectionFlags = playerr["lane_selection_flags"].GetValue<uint>();
                                player.recentMatches[matchNumber].lastHits = playerr["last_hits"].GetValue<uint>();
                                player.recentMatches[matchNumber].denies = playerr["denies"].GetValue<uint>();
                                break;
                            }
                        }
                        break;
                    }
            }
        }

        private async void UpdateManager_Update()
        {
            //Profile requests
            for (int k = 0; k < profileRequests.Count; k++)
            {
                if (profileRequestsSleeper.Sleeping || profileRequests[k].sent)
                    continue;
                NetworkManager.SendGCMessageProfileRequest(profileRequests[k].accountId, profileRequests[k].jobId);
                profileRequests[k].sent = true;
                profileRequests.Remove(profileRequests[k]);
                profileRequestsSent++;
                //Console.WriteLine("Profile requests Sent: " + profileRequestsSent);


                //Profile requests delay
                if (profileRequestsSent % 4 == 0)
                {
                    //Console.WriteLine("Profile request delay 4001ms");
                    //Console.WriteLine("Profile requests Sent: " + profileRequestsSent);
                    //Console.WriteLine("Proto requests Sent: " + (profileRequestsSent + matchDetailsRequestsSent));
                    profileRequestsSleeper.Sleep(4001);
                }
                //
                break;
            }
            //

            //Match details requests
            //for (int k = 0; k < matchDetailsRequests.Count; k++)
            //{
            //    if (matchDetailsRequestsSleeper.Sleeping || matchDetailsRequests[k].sent)
            //        continue;
            //    NetworkManager.SendGCMatchDetailsRequest(matchDetailsRequests[k].matchId, matchDetailsRequests[k].jobId);
            //    matchDetailsRequests[k].sent = true;
            //    matchDetailsRequests.Remove(matchDetailsRequests[k]);
            //    matchDetailsRequestsSent++;
            //    Console.WriteLine("Match details requests Sent: " + matchDetailsRequestsSent);


            //    //Match details requests delay
            //    if (matchDetailsRequestsSent % 4 == 0)
            //    {
            //        Console.WriteLine("Match details request delay 4001ms");
            //        Console.WriteLine("Match details requests Sent: " + matchDetailsRequestsSent);
            //        Console.WriteLine("Proto requests Sent: " + (profileRequestsSent + matchDetailsRequestsSent));
            //        matchDetailsRequestsSleeper.Sleep(4001);
            //    }
            //    //
            //    break;
            //}
            //

            //Http requests
            for (int i = 0; i < httpRequests.Count; i++)
            {
                switch (httpRequests[i].requestType)
                {
                    case RequestType.GetDivineStratzData:
                        {
                            if (httpRequests[i].sent)
                                continue;
                            httpRequests[i].sent = true;
                            var stratzData = await HttpRequests.GetDivineStratzDataAsync(
                                httpRequests[i].accountId,
                                httpRequests[i].startDateTime).ConfigureAwait(false);
                            if (stratzData == null) continue;

                            var playerNumber = httpRequests[i].playerNumber;
                            //Console.WriteLine("id: " + stratzData?.mainInfo?.steamAccount?.id);
                            //Console.WriteLine("isAnonymous: " + stratzData?.mainInfo?.steamAccount?.isAnonymous);
                            playerTable[playerNumber].isAnonymous = stratzData.mainInfo.steamAccount.isAnonymous;
                            if (!playerTable[playerNumber].isAnonymous)
                            {
                                playerTable[playerNumber].matchCount = (uint)stratzData.mainInfo.matchCount;
                                playerTable[playerNumber].winPercent = (uint)Math.Round(((float)stratzData.mainInfo.winCount / (float)stratzData.mainInfo.matchCount) * 100f);
                                var orderedHeroPerformance = stratzData.heroPerformance.OrderByDescending(x => x.matchCount).ToList();
                                for (int k = 0; k < 3; k++)
                                {
                                    if (orderedHeroPerformance.Count < (k + 1)) continue;
                                    playerTable[playerNumber].mostPlayed[k] = orderedHeroPerformance[k];
                                }
                            }
                            break;
                        }
                    case RequestType.GetMatchDetails:
                        {
                            if (matchDetailsRequestsSleeper.Sleeping || httpRequests[i].sent)
                                continue;
                            httpRequests[i].sent = true;
                            matchDetailsRequestsSent++;
                            if (matchDetailsRequestsSent % 9 == 0)
                            {
                                Console.WriteLine("===================================================================");
                                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff:fff")} | GetMatchDetails for {httpRequests[i].matchId} Delay 500ms");
                                Console.WriteLine("===================================================================");
                                matchDetailsRequestsSleeper.Sleep(500);
                            }
                            var responseMessage = await HttpRequests.GetMatchDetailsAsync(httpRequests[i].matchId).ConfigureAwait(false);

                            MatchDetails matchDetails = null;
                            if (responseMessage.IsSuccessStatusCode)
                            {
                                matchDetails = await responseMessage.Content.ReadFromJsonAsync<MatchDetails>().ConfigureAwait(false);
                            }
                            else if ((int)responseMessage.StatusCode == 429)
                            {
                                httpRequests.Add(new HttpMessage
                                {
                                    requestType = RequestType.GetMatchDetails,
                                    matchId = httpRequests[i].matchId,
                                    playerNumber = httpRequests[i].playerNumber,
                                    matchNumber = httpRequests[i].matchNumber,
                                });
                                Console.WriteLine("===================================================================");
                                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} | GetMatchDetails for {httpRequests[i].matchId} Delay 500ms");
                                Console.WriteLine("===================================================================");
                                matchDetailsRequestsSleeper.Sleep(500);
                                continue;
                            }

                            if (matchDetails == null || matchDetails.result.error != null || matchDetails.result.players == null) continue;

                            var playerNumber = httpRequests[i].playerNumber;
                            var matchNumber = httpRequests[i].matchNumber;

                            playerTable[playerNumber].recentMatches[matchNumber].duration = (uint)matchDetails.result.duration;
                            foreach (var player in matchDetails.result.players)
                            {
                                if (player.hero_id == (int)playerTable[playerNumber].recentMatches[matchNumber].heroId)
                                {
                                    playerTable[playerNumber].recentMatches[matchNumber].kills = (uint)player.kills;
                                    playerTable[playerNumber].recentMatches[matchNumber].deaths = (uint)player.deaths;
                                    playerTable[playerNumber].recentMatches[matchNumber].assists = (uint)player.assists;
                                    playerTable[playerNumber].recentMatches[matchNumber].XPPerMin = (uint)player.xp_per_min;
                                    playerTable[playerNumber].recentMatches[matchNumber].goldPerMin = (uint)player.gold_per_min;
                                    playerTable[playerNumber].recentMatches[matchNumber].lastHits = (uint)player.last_hits;
                                    playerTable[playerNumber].recentMatches[matchNumber].denies = (uint)player.denies;
                                }
                            }
                            break;
                        }
                }
                break;
            }
            //
        }

        private void NetworkManager_GCSOMessageUpdate(GCSOMessageUpdateEventArgs e)
        {
            var lobbyDataProto = e.Protobuf;
            if (lobbyDataProto.MessageId != GCSOMessageId.CSODOTALobby) return;

            var lobbyDataJson = lobbyDataProto.ToJson();
            var members = lobbyDataJson["all_members"].AsArray();

            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (!playerTable.ContainsKey(i))
                    playerTable.Add(i, new PlayerData());

                if (ulong.TryParse(member["id"].GetValue<string>(), out var id))
                    playerTable[i].id = SteamId64to32(id);
                if (ulong.TryParse(member["party_id"].GetValue<string>(), out var party_id))
                    playerTable[i].partyId = party_id;

                playerTable[i].heroId = (HeroId)member["hero_id"].GetValue<uint>();
                playerTable[i].name = member["name"].GetValue<string>().Windows1251ToUtf8();
                playerTable[i].laneSelectonFlags = (LaneSelectonFlags)member["lane_selection_flags"].GetValue<int>();
                playerTable[i].rankTier = member["rank_tier"].GetValue<int>();

                profileRequests.Add(new ProtoMessage
                {
                    msgId = GCMessageId.CMsgProfileRequest,
                    accountId = playerTable[i].id,
                    jobId = (ulong)(i + 1)
                });
                httpRequests.Add(new HttpMessage
                {
                    requestType = RequestType.GetDivineStratzData,
                    accountId = playerTable[i].id,
                    playerNumber = i,
                    startDateTime = DateTime.UtcNow.AddMonths(-1)
                });
                //NetworkManager.SendGCMessageProfileRequest(playerTable[i].id, (ulong)(i + 1));
                //NetworkManager.SendGCClientToGCRequestPlayerRecentAccomplishments(playerTable[i].id, (ulong)(i + 1));
                //DateTime.Today.AddDays(-DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))
            }
            Console.WriteLine(DateTime.UtcNow);
        }

        private uint SteamId64to32(ulong steamId64)
        {
            return (uint)(steamId64 & 0xFFFFFFFF);
        }

        public void Dispose()
        {
        }
    }
}
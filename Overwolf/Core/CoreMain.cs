using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Network;
using Divine.Network.EventArgs;
using Divine.Network.GC;
using Divine.Network.GCSO;
using Divine.Update;
using Divine.Zero.Log;

using Overwolf.Exstensions;
using Overwolf.Helpers;
using Overwolf.Requests.Http;
using Overwolf.Requests.Http.Models.SteamAPI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;

using static Overwolf.Data.Data;

namespace Overwolf.Core
{
    internal sealed class CoreMain
    {
        private readonly Context Context;
        private readonly Menu MainMenu;
        internal Dictionary<int, PlayerData> playerTable = new Dictionary<int, PlayerData>();
        private List<HttpMessage> httpRequests = new List<HttpMessage>();
        private List<ProtoMessage> profileRequests = new List<ProtoMessage>();
        private uint profileRequestsSent = 0;
        private Sleeper profileRequestsSleeper = new Sleeper();
        private uint httpRequestsSent = 0;
        private Sleeper httpRequestsSleeper = new Sleeper();
        private List<TempClass> TempTable = new List<TempClass>()
        {
            //new TempClass { id = 179035658, partyId = 123123, heroId = 12, name = "ks", laneSelectonFlags = LaneSelectonFlags.hardsupport}, //1
            //new TempClass { id = 136829091, partyId = 123123, heroId = 17, name = "Angel ", laneSelectonFlags = LaneSelectonFlags.safelane},
            //new TempClass { id = 152545459, partyId = 454545, heroId = 26, name = "Greedy", laneSelectonFlags = LaneSelectonFlags.softsupport},
            //new TempClass { id = 293904640, partyId = 454545, heroId = 50, name = "Hitman", laneSelectonFlags = LaneSelectonFlags.midlane},
            //new TempClass { id = 422748003, partyId = 454545, heroId = 76, name = "Davai Lama", laneSelectonFlags = LaneSelectonFlags.offlane},
            //new TempClass { id = 206642367, partyId = 999898, heroId = 83, name = "tw.tv/SpeedManq", laneSelectonFlags = LaneSelectonFlags.offlane},
            //new TempClass { id = 198366226, partyId = 999898, heroId = 31, name = "Pablo", laneSelectonFlags = LaneSelectonFlags.softsupport},
            //new TempClass { id = 916650633, partyId = 345345, heroId = 49, name = "Save-", laneSelectonFlags = LaneSelectonFlags.hardsupport},
            //new TempClass { id = 197104697, partyId = 345345, heroId = 22, name = "monkemajor", laneSelectonFlags = LaneSelectonFlags.midlane},
            //new TempClass { id = 301750126, partyId = 345345, heroId = 66, name = "Danial", laneSelectonFlags = LaneSelectonFlags.safelane}, // 10
            //new TempClass { id = 182304190, partyId = 123123, heroId = 12, name = "ks", laneSelectonFlags = LaneSelectonFlags.hardsupport}, //1
            //new TempClass { id = 352031777, partyId = 123123, heroId = 17, name = "Angel ", laneSelectonFlags = LaneSelectonFlags.safelane},
            //new TempClass { id = 452400903, partyId = 454545, heroId = 26, name = "Greedy", laneSelectonFlags = LaneSelectonFlags.softsupport},
            //new TempClass { id = 493967542, partyId = 454545, heroId = 50, name = "Hitman", laneSelectonFlags = LaneSelectonFlags.midlane},
            //new TempClass { id = 924944185, partyId = 454545, heroId = 76, name = "Davai Lama", laneSelectonFlags = LaneSelectonFlags.offlane},
            //new TempClass { id = 103656457, partyId = 999898, heroId = 83, name = "tw.tv/SpeedManq", laneSelectonFlags = LaneSelectonFlags.offlane},
            //new TempClass { id = 117311875, partyId = 999898, heroId = 31, name = "Pablo", laneSelectonFlags = LaneSelectonFlags.softsupport},
            //new TempClass { id = 171498131, partyId = 345345, heroId = 49, name = "Save-", laneSelectonFlags = LaneSelectonFlags.hardsupport},
            //new TempClass { id = 216264942, partyId = 345345, heroId = 22, name = "monkemajor", laneSelectonFlags = LaneSelectonFlags.midlane},
            //new TempClass { id = 218172539, partyId = 345345, heroId = 66, name = "Danial", laneSelectonFlags = LaneSelectonFlags.safelane}, // 10
            new TempClass { id = 120303454, partyId = 123123, heroId = 12, name = "ks", laneSelectonFlags = LaneSelectonFlags.hardsupport}, // 1
            new TempClass { id = 276901240, partyId = 123123, heroId = 17, name = "Angel ", laneSelectonFlags = LaneSelectonFlags.safelane},
            new TempClass { id = 109455705, partyId = 454545, heroId = 26, name = "Greedy", laneSelectonFlags = LaneSelectonFlags.softsupport},
            new TempClass { id = 107562000, partyId = 454545, heroId = 50, name = "Hitman", laneSelectonFlags = LaneSelectonFlags.midlane},
            new TempClass { id = 138880576, partyId = 454545, heroId = 76, name = "Davai Lama", laneSelectonFlags = LaneSelectonFlags.offlane},
            new TempClass { id = 1060164724, partyId = 999898, heroId = 83, name = "tw.tv/SpeedManq", laneSelectonFlags = LaneSelectonFlags.offlane},
            new TempClass { id = 117015167, partyId = 999898, heroId = 31, name = "Pablo", laneSelectonFlags = LaneSelectonFlags.softsupport},
            new TempClass { id = 317880638, partyId = 345345, heroId = 49, name = "Save-", laneSelectonFlags = LaneSelectonFlags.hardsupport},
            new TempClass { id = 52797075, partyId = 345345, heroId = 22, name = "monkemajor", laneSelectonFlags = LaneSelectonFlags.midlane},
            new TempClass { id = 120117384, partyId = 345345, heroId = 66, name = "Danial", laneSelectonFlags = LaneSelectonFlags.safelane}, //10
        };

        internal CoreMain(Context context)
        {
            Context = context;
            MainMenu = Context.Menu;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            MainMenu.OverwolfSwitcher.ValueChanged += OverwolfSwitcher_ValueChanged;

            //for (int i = 0; i < 10; i++)
            //{
            //    if (!playerTable.ContainsKey(i))
            //        playerTable.Add(i, new PlayerData());

            //    playerTable[i].id = TempTable[i].id;
            //    playerTable[i].partyId = TempTable[i].partyId;
            //    playerTable[i].heroId = (HeroId)TempTable[i].heroId;
            //    playerTable[i].name = TempTable[i].name;
            //    playerTable[i].laneSelectonFlags = TempTable[i].laneSelectonFlags;

            //    if (i < 9)
            //    {
            //        if (TempTable[i].partyId == TempTable[i + 1].partyId)
            //        {
            //            playerTable[i].partyWithNextPlayer = true;
            //        }
            //    }

            //    profileRequests.Add(new ProtoMessage
            //    {
            //        msgId = GCMessageId.CMsgProfileRequest,
            //        accountId = playerTable[i].id,
            //        jobId = (ulong)(i + 1)
            //    });

            //    httpRequests.Add(new HttpMessage
            //    {
            //        requestType = RequestType.GetDivineStratzData,
            //        accountId = playerTable[i].id,
            //        playerNumber = i,
            //        startDateTime = DateTime.UtcNow.AddMonths(-1)
            //    });
            //}
        }

        private void OverwolfSwitcher_ValueChanged(Divine.Menu.Items.MenuSwitcher switcher, Divine.Menu.EventArgs.SwitcherEventArgs e)
        {
            if (e.Value)
            {
                UpdateManager.Update += UpdateManager_Update;
                NetworkManager.GCSOMessageUpdate += NetworkManager_GCSOMessageUpdate;
                NetworkManager.GCMessageReceived += NetworkManager_GCMessageReceived;
            }
            else
            {
                UpdateManager.Update -= UpdateManager_Update;
                NetworkManager.GCSOMessageUpdate -= NetworkManager_GCSOMessageUpdate;
                NetworkManager.GCMessageReceived -= NetworkManager_GCMessageReceived;
            }
        }

        internal class TempClass
        {
            internal uint id;
            internal ulong partyId;
            internal int heroId;
            internal string name;
            internal LaneSelectonFlags laneSelectonFlags;
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
            LogManager.Debug($"{e.Protobuf.MessageId} JobId: {e.JobId} Result: {e.Protobuf.ToJson()["result"]}");
            //Console.WriteLine(e.Protobuf.ToJson());

            var jobId = (int)(e.JobId - 1);
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
                            //if (!profileRequestsSleeper.Sleeping)
                            //{
                            //    LogManager.Debug($"ProfileRequest Delay for {player.id} Delay 1000ms");
                            //    profileRequestsSleeper.Sleep(1000);
                            //}
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
                                requestType = RequestType.GetDivineMatchDetails,
                                matchId = match_id,
                                playerNumber = playerId,
                                matchNumber = index,
                            });

                            player.recentMatches[index].matchTimestamp = match["match_timestamp"].GetValue<uint>();
                            player.recentMatches[index].performanceRating = match["performance_rating"].GetValue<int>();
                            player.recentMatches[index].heroId = (HeroId)match["hero_id"].GetValue<uint>();
                            player.recentMatches[index].wonMatch = match["won_match"].GetValue<bool>();
                        }
                        break;
                    }
            }
        }

        private async void UpdateManager_Update()
        {
            //Profile requests
            for (int i = 0; i < profileRequests.Count; i++)
            {
                if (profileRequestsSleeper.Sleeping || profileRequests[i].sent)
                    continue;
                profileRequests[i].sent = true;

                NetworkManager.SendGCMessageProfileRequest(profileRequests[i].accountId, profileRequests[i].jobId);
                LogManager.Debug($"ProfileRequest for {profileRequests[i].accountId} Sent {profileRequestsSent}");

                profileRequestsSent++;
                if (profileRequestsSent % 5 == 0)
                {
                    LogManager.Debug($"ProfileRequest Delay for {profileRequests[i].accountId} Delay 3000ms");
                    profileRequestsSleeper.Sleep(3000);
                }
                else if (profileRequestsSent % 1 == 0)
                {
                    LogManager.Debug($"ProfileRequest Delay for {profileRequests[i].accountId} Delay 200ms");
                    profileRequestsSleeper.Sleep(200);
                }
                break;
            }
            //

            //Http requests
            for (int i = 0; i < httpRequests.Count; i++)
            {
                switch (httpRequests[i].requestType)
                {
                    case RequestType.GetDivineStratzData:
                        {
                            if (httpRequestsSleeper.Sleeping || httpRequests[i].sent)
                                continue;
                            httpRequests[i].sent = true;
                            httpRequestsSent++;
                            if (httpRequestsSent % 1 == 0)
                            {
                                LogManager.Debug($"httpRequests Delay for {httpRequests[i].accountId} Delay 100ms");
                                httpRequestsSleeper.Sleep(100);
                            }

                            LogManager.Debug($"GetDivineStratzData for {httpRequests[i].accountId} Sent");
                            var stratzData = await HttpRequests.GetDivineStratzDataAsync(
                                httpRequests[i].accountId,
                                httpRequests[i].startDateTime).ConfigureAwait(false);
                            if (stratzData == null) continue;

                            var playerNumber = httpRequests[i].playerNumber;

                            playerTable[playerNumber].isAnonymous = stratzData?.mainInfo?.steamAccount?.isAnonymous ?? true;
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
                    case RequestType.GetDivineMatchDetails:
                        {
                            if (httpRequestsSleeper.Sleeping || httpRequests[i].sent)
                                continue;
                            httpRequests[i].sent = true;
                            httpRequestsSent++;
                            if (httpRequestsSent % 1 == 0)
                            {
                                LogManager.Debug($"httpRequests Delay for {httpRequests[i].matchId} Delay 100ms");
                                httpRequestsSleeper.Sleep(100);
                            }

                            LogManager.Debug($"GetDivineMatchDetails for {httpRequests[i].matchId} Sent");
                            var matchDetails = await HttpRequests.GetDivineMatchDetailsAsync(httpRequests[i].matchId).ConfigureAwait(false);

                            if (matchDetails == null || matchDetails?.result?.error != null || matchDetails?.result?.players == null) continue;

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
                                    break;
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

            if (!Context.Menu.OverwolfToggleKey.Value)
                Context.Menu.OverwolfToggleKey.Value = true;

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

                if (i < (members.Count - 1))
                {
                    if (ulong.TryParse(members[i + 1]["party_id"].GetValue<string>(), out var nextParty_id)
                        && party_id == nextParty_id)
                    {
                        playerTable[i].partyWithNextPlayer = true;
                    }
                }

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
                    startDateTime = DateTime.Today.AddMonths(-1)
                });
            }
        }

        private uint SteamId64to32(ulong steamId64)
        {
            return (uint)(steamId64 & 0xFFFFFFFF);
        }

        internal void Dispose()
        {
        }
    }
}
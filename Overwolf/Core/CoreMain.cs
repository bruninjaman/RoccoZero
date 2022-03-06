using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Game;
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
using System.Text.Json;

using static Overwolf.Data.Data;

namespace Overwolf.Core
{
    internal sealed class CoreMain
    {
        internal Dictionary<int, PlayerData> _playerTable = new Dictionary<int, PlayerData>();
        private readonly Context _context;
        private readonly Menu _mainMenu;
        private List<ProtoMessage> _profileRequests = new List<ProtoMessage>();
        private uint _profileRequestsSent = 0;
        private Sleeper _profileRequestsSleeper = new Sleeper();
        private List<HttpMessage> _httpRequests = new List<HttpMessage>();
        private uint _httpRequestsSent = 0;
        private Sleeper _httpRequestsSleeper = new Sleeper();
        private List<string> _requestPlayers = new List<string>();
        private bool _isInLobby = true;
        //private List<TempClass> _tempTable = new List<TempClass>()
        //{
        //    new TempClass { id = 34505203/*179035658*/, partyId = 123123, heroId = 12, name = "ks", laneSelectonFlags = LaneSelectonFlags.hardsupport}, //1
        //    new TempClass { id = 136829091, partyId = 123123, heroId = 17, name = "Angel ", laneSelectonFlags = LaneSelectonFlags.safelane},
        //    new TempClass { id = 163737155/*152545459*/, partyId = 454545, heroId = 26, name = "Greedy", laneSelectonFlags = LaneSelectonFlags.softsupport},
        //    new TempClass { id = 431770905/*293904640*/, partyId = 454545, heroId = 50, name = "Hitman", laneSelectonFlags = LaneSelectonFlags.midlane},
        //    new TempClass { id = 422748003, partyId = 454545, heroId = 76, name = "Davai Lama", laneSelectonFlags = LaneSelectonFlags.offlane},
        //    new TempClass { id = 206642367, partyId = 999898, heroId = 83, name = "tw.tv/SpeedManq", laneSelectonFlags = LaneSelectonFlags.offlane},
        //    new TempClass { id = 159020918/*198366226*/, partyId = 999898, heroId = 31, name = "Pablo", laneSelectonFlags = LaneSelectonFlags.softsupport},
        //    new TempClass { id = 82262664/*916650633*/, partyId = 345345, heroId = 49, name = "Save-", laneSelectonFlags = LaneSelectonFlags.hardsupport},
        //    new TempClass { id = 197104697, partyId = 345345, heroId = 22, name = "monkemajor", laneSelectonFlags = LaneSelectonFlags.midlane},
        //    new TempClass { id = 301750126, partyId = 345345, heroId = 66, name = "Danial", laneSelectonFlags = LaneSelectonFlags.safelane}, // 10
        //    //new TempClass { id = 182304190, partyId = 123123, heroId = 12, name = "ks", laneSelectonFlags = LaneSelectonFlags.hardsupport}, //1
        //    //new TempClass { id = 352031777, partyId = 123123, heroId = 17, name = "Angel ", laneSelectonFlags = LaneSelectonFlags.safelane},
        //    //new TempClass { id = 452400903, partyId = 454545, heroId = 26, name = "Greedy", laneSelectonFlags = LaneSelectonFlags.softsupport},
        //    //new TempClass { id = 493967542, partyId = 454545, heroId = 50, name = "Hitman", laneSelectonFlags = LaneSelectonFlags.midlane},
        //    //new TempClass { id = 924944185, partyId = 454545, heroId = 76, name = "Davai Lama", laneSelectonFlags = LaneSelectonFlags.offlane},
        //    //new TempClass { id = 103656457, partyId = 999898, heroId = 83, name = "tw.tv/SpeedManq", laneSelectonFlags = LaneSelectonFlags.offlane},
        //    //new TempClass { id = 117311875, partyId = 999898, heroId = 31, name = "Pablo", laneSelectonFlags = LaneSelectonFlags.softsupport},
        //    //new TempClass { id = 171498131, partyId = 345345, heroId = 49, name = "Save-", laneSelectonFlags = LaneSelectonFlags.hardsupport},
        //    //new TempClass { id = 216264942, partyId = 345345, heroId = 22, name = "monkemajor", laneSelectonFlags = LaneSelectonFlags.midlane},
        //    //new TempClass { id = 218172539, partyId = 345345, heroId = 66, name = "Danial", laneSelectonFlags = LaneSelectonFlags.safelane}, // 10
        //    //new TempClass { id = 120303454, partyId = 123123, heroId = 12, name = "ks", laneSelectonFlags = LaneSelectonFlags.hardsupport}, // 1
        //    //new TempClass { id = 276901240, partyId = 123123, heroId = 17, name = "Angel ", laneSelectonFlags = LaneSelectonFlags.safelane},
        //    //new TempClass { id = 109455705, partyId = 454545, heroId = 26, name = "Greedy", laneSelectonFlags = LaneSelectonFlags.softsupport},
        //    //new TempClass { id = 107562000, partyId = 454545, heroId = 50, name = "Hitman", laneSelectonFlags = LaneSelectonFlags.midlane},
        //    //new TempClass { id = 138880576, partyId = 454545, heroId = 76, name = "Davai Lama", laneSelectonFlags = LaneSelectonFlags.offlane},
        //    //new TempClass { id = 1060164724, partyId = 999898, heroId = 83, name = "tw.tv/SpeedManq", laneSelectonFlags = LaneSelectonFlags.offlane},
        //    //new TempClass { id = 117015167, partyId = 999898, heroId = 31, name = "Pablo", laneSelectonFlags = LaneSelectonFlags.softsupport},
        //    //new TempClass { id = 317880638, partyId = 345345, heroId = 49, name = "Save-", laneSelectonFlags = LaneSelectonFlags.hardsupport},
        //    //new TempClass { id = 52797075, partyId = 345345, heroId = 22, name = "monkemajor", laneSelectonFlags = LaneSelectonFlags.midlane},
        //    //new TempClass { id = 120117384, partyId = 345345, heroId = 66, name = "Danial", laneSelectonFlags = LaneSelectonFlags.safelane}, //10
        //};
        private bool _blockUpdate = false;

        internal CoreMain(Context context)
        {
            _context = context;
            _mainMenu = _context.Menu;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _mainMenu.OverwolfSwitcher.ValueChanged += OverwolfSwitcher_ValueChanged;

            //for (int i = 0; i < 10; i++)
            //{
            //    if (!_playerTable.ContainsKey(i))
            //        _playerTable.Add(i, new PlayerData());

            //    _playerTable[i].id = _tempTable[i].id;
            //    _playerTable[i].partyId = _tempTable[i].partyId;
            //    _playerTable[i].heroId = (HeroId)_tempTable[i].heroId;
            //    _playerTable[i].name = _tempTable[i].name;
            //    _playerTable[i].laneSelectonFlags = _tempTable[i].laneSelectonFlags;

            //    if (i < 9)
            //    {
            //        if (_tempTable[i].partyId == _tempTable[i + 1].partyId)
            //        {
            //            _playerTable[i].partyWithNextPlayer = true;
            //        }
            //    }
            //    _requestPlayers.Add(_playerTable[i].id.ToString());

            //    if (i == 4 || i == 9)
            //    {
            //        _httpRequests.Add(new HttpMessage
            //        {
            //            requestType = RequestType.GetDivineStratzGraphQLData,
            //            players = _requestPlayers.ToList(),
            //            startDateTime = DateTime.UtcNow.AddMonths(-1)
            //        });
            //        _requestPlayers.Clear();
            //    }

            //    //_profileRequests.Add(new ProtoMessage
            //    //{
            //    //    msgId = GCMessageId.CMsgProfileRequest,
            //    //    accountId = _playerTable[i].id,
            //    //    jobId = (ulong)(i + 1)
            //    //});

            //    //httpRequests.Add(new HttpMessage
            //    //{
            //    //    requestType = RequestType.GetDivineStratzData,
            //    //    accountId = playerTable[i].id,
            //    //    playerNumber = i,
            //    //    startDateTime = DateTime.UtcNow.AddMonths(-1)
            //    //});
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

        //internal class TempClass
        //{
        //    internal uint id;
        //    internal ulong partyId;
        //    internal int heroId;
        //    internal string name;
        //    internal LaneSelectonFlags laneSelectonFlags;
        //}

        private void NetworkManager_GCMessageReceived(GCMessageReceivedEventArgs e)
        {
            if (e.JobId >= 100 && (e.Protobuf.MessageId != GCMessageId.CMsgProfileResponse)
                && (e.Protobuf.MessageId != GCMessageId.CMsgSOCacheUnsubscribed)
                && (e.Protobuf.MessageId != GCMessageId.CMsgSOCacheSubscribed)
                && (e.Protobuf.MessageId != (GCMessageId)26))
                return;

            _blockUpdate = e.Protobuf.MessageId == (GCMessageId)26;

            if (_blockUpdate)
                return;

            if (e.Protobuf.MessageId == GCMessageId.CMsgSOCacheSubscribed
                && e.Protobuf.ToJson()["owner_soid"]["type"].GetValue<int>() == 3)
            {
                _isInLobby = true;
            }

            if (e.Protobuf.MessageId == GCMessageId.CMsgSOCacheUnsubscribed
                && e.Protobuf.ToJson()["owner_soid"]["type"].GetValue<int>() == 3)
            {
                _isInLobby = false;
            }

            //LogManager.Debug($"{e.Protobuf.MessageId} JobId: {e.JobId} Result: {e.Protobuf.ToJson()["result"] ?? ""}");

            var jobId = (int)(e.JobId - 1);
            var playerId = jobId % 10;
            if (!_playerTable.ContainsKey(playerId)) return;
            var player = _playerTable[playerId];

            if (e.Protobuf.ToJson()["result"].GetValue<string>() == "k_eTooBusy")
            {
                _profileRequests.Add(new ProtoMessage
                {
                    msgId = GCMessageId.CMsgProfileRequest,
                    accountId = player.id,
                    jobId = e.JobId
                });
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

                _httpRequests.Add(new HttpMessage
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
        }

        private async void UpdateManager_Update()
        {

            //Profile requests
            if (!_isInLobby)
            {
                _playerTable.Clear();
            }

            for (int i = 0; i < _profileRequests.Count; i++)
            {
                if (_profileRequestsSleeper.Sleeping || _profileRequests[i].sent)
                    continue;
                _profileRequests[i].sent = true;

                NetworkManager.SendGCMessageProfileRequest(_profileRequests[i].accountId, _profileRequests[i].jobId);
                LogManager.Debug($"ProfileRequest for {_profileRequests[i].accountId} Sent {_profileRequestsSent}");

                _profileRequestsSent++;
                if (_profileRequestsSent % 1 == 0)
                {
                    LogManager.Debug($"ProfileRequest Delay for {_profileRequests[i].accountId} Delay 200ms");
                    _profileRequestsSleeper.Sleep(200);
                }
                break;
            }
            //

            //Http requests
            for (int i = 0; i < _httpRequests.Count; i++)
            {
                switch (_httpRequests[i].requestType)
                {
                    case RequestType.GetDivineStratzGraphQLData:
                        {
                            if (_httpRequestsSleeper.Sleeping || _httpRequests[i].sent)
                                continue;

                            _httpRequests[i].sent = true;
                            _httpRequestsSent++;
                            if (_httpRequestsSent % 1 == 0)
                            {
                                LogManager.Debug($"httpRequests Delay Delay 100ms");
                                _httpRequestsSleeper.Sleep(100);
                            }

                            LogManager.Debug($"GetDivineStratzGraphQLData Sent");
                            var stratzGraphQLData = await HttpRequests.GetDivineStratzGraphQLDataAsync(
                                _httpRequests[i].players,
                                _httpRequests[i].startDateTime).ConfigureAwait(false);
                            if (stratzGraphQLData == null) continue;

                            foreach (var playerData in stratzGraphQLData.players)
                            {
                                foreach  (var (index, player) in _playerTable)
                                {
                                    if ((uint)playerData.steamAccount.id == player.id)
                                    {

                                        player.isAnonymous = playerData?.steamAccount?.isAnonymous ?? true;
                                        if (player.isAnonymous)
                                        {
                                            _profileRequests.Add(new ProtoMessage
                                            {
                                                msgId = GCMessageId.CMsgProfileRequest,
                                                accountId = player.id,
                                                jobId = (ulong)(index + 1)
                                            });
                                            continue;
                                        }

                                        foreach (var heroesPerformance in playerData.heroesPerformance)
                                        {
                                            player.matchCount += (uint)heroesPerformance.matchCount;
                                        }
                                        //player.matchCount = (uint)playerData.matchCount;

                                        player.winPercent = (uint)Math.Round(((float)playerData.winCount / (float)playerData.matchCount) * 100f);
                                        var orderedHeroPerformance = playerData.heroesPerformance.OrderByDescending(x => x.matchCount).ToList();
                                        for (int k = 0; k < 3; k++)
                                        {
                                            if (orderedHeroPerformance.Count < (k + 1)) continue;
                                            player.mostPlayed[k] = orderedHeroPerformance[k];
                                        }
                                        for (int k = 0; k < 8; k++)
                                        {
                                            if (!player.recentMatches.ContainsKey(k))
                                                player.recentMatches.Add(k, new Match());

                                            player.recentMatches[k].matchId = playerData.matches[k].id ?? 0;
                                            player.recentMatches[k].heroId = playerData.matches[k].players[0].heroId ?? 0;
                                            player.recentMatches[k].duration = (uint)playerData.matches[k].durationSeconds;
                                            player.recentMatches[k].performanceRating = playerData.matches[k].players[0].imp ?? 0;
                                            player.recentMatches[k].matchTimestamp = (uint)playerData.matches[k].endDateTime;
                                            player.recentMatches[k].wonMatch = (playerData.matches[k].players[0].isRadiant ?? true) ? playerData.matches[k].didRadiantWin ?? true : !playerData.matches[k].didRadiantWin ?? true;
                                            player.recentMatches[k].kills = (uint)playerData.matches[k].players[0].kills;
                                            player.recentMatches[k].deaths = (uint)playerData.matches[k].players[0].deaths;
                                            player.recentMatches[k].assists = (uint)playerData.matches[k].players[0].assists;
                                            player.recentMatches[k].XPPerMin = (uint)playerData.matches[k].players[0].experiencePerMinute;
                                            player.recentMatches[k].goldPerMin = (uint)playerData.matches[k].players[0].goldPerMinute;
                                            player.recentMatches[k].lastHits = (uint)playerData.matches[k].players[0].numLastHits;
                                            player.recentMatches[k].denies = (uint)playerData.matches[k].players[0].numDenies;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case RequestType.GetDivineMatchDetails:
                        {
                            if (_httpRequestsSleeper.Sleeping || _httpRequests[i].sent)
                                continue;
                            _httpRequests[i].sent = true;
                            _httpRequestsSent++;
                            if (_httpRequestsSent % 1 == 0)
                            {
                                LogManager.Debug($"httpRequests Delay for {_httpRequests[i].matchId} Delay 100ms");
                                _httpRequestsSleeper.Sleep(100);
                            }

                            LogManager.Debug($"GetDivineMatchDetails for {_httpRequests[i].matchId} Sent");
                            var matchDetails = await HttpRequests.GetDivineMatchDetailsAsync(_httpRequests[i].matchId).ConfigureAwait(false);

                            if (matchDetails == null || matchDetails?.result?.error != null || matchDetails?.result?.players == null) continue;

                            var playerNumber = _httpRequests[i].playerNumber;
                            var matchNumber = _httpRequests[i].matchNumber;

                            _playerTable[playerNumber].recentMatches[matchNumber].duration = (uint)matchDetails.result.duration;
                            foreach (var player in matchDetails.result.players)
                            {
                                if (player.hero_id == (int)_playerTable[playerNumber].recentMatches[matchNumber].heroId)
                                {
                                    _playerTable[playerNumber].recentMatches[matchNumber].kills = (uint)player.kills;
                                    _playerTable[playerNumber].recentMatches[matchNumber].deaths = (uint)player.deaths;
                                    _playerTable[playerNumber].recentMatches[matchNumber].assists = (uint)player.assists;
                                    _playerTable[playerNumber].recentMatches[matchNumber].XPPerMin = (uint)player.xp_per_min;
                                    _playerTable[playerNumber].recentMatches[matchNumber].goldPerMin = (uint)player.gold_per_min;
                                    _playerTable[playerNumber].recentMatches[matchNumber].lastHits = (uint)player.last_hits;
                                    _playerTable[playerNumber].recentMatches[matchNumber].denies = (uint)player.denies;
                                    break;
                                }
                            }
                            break;
                        }
                }
            }
            //
        }

        private void NetworkManager_GCSOMessageUpdate(GCSOMessageUpdateEventArgs e)
        {
            if (_blockUpdate)
                return;

            var lobbyDataProto = e.Protobuf;
            if (lobbyDataProto.MessageId != GCSOMessageId.CSODOTALobby) return;

            if (!_context.Menu.OverwolfToggleKey.Value)
                _context.Menu.OverwolfToggleKey.Value = true;

            var lobbyDataJson = lobbyDataProto.ToJson();
            var members = lobbyDataJson["all_members"].AsArray();

            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (!_playerTable.ContainsKey(i))
                    _playerTable.Add(i, new PlayerData());

                if (ulong.TryParse(member["id"].GetValue<string>(), out var id))
                    _playerTable[i].id = SteamId64to32(id);
                if (ulong.TryParse(member["party_id"].GetValue<string>(), out var party_id))
                    _playerTable[i].partyId = party_id;

                if (i < (members.Count - 1))
                {
                    if (ulong.TryParse(members[i + 1]["party_id"].GetValue<string>(), out var nextParty_id)
                        && party_id == nextParty_id)
                    {
                        _playerTable[i].partyWithNextPlayer = true;
                    }
                }

                _playerTable[i].heroId = (HeroId)member["hero_id"].GetValue<uint>();
                _playerTable[i].name = member["name"].GetValue<string>().Windows1251ToUtf8();
                _playerTable[i].laneSelectonFlags = (LaneSelectonFlags)member["lane_selection_flags"].GetValue<int>();
                _playerTable[i].rankTier = member["rank_tier"].GetValue<int>();


                _requestPlayers.Add(_playerTable[i].id.ToString());
                if (((members.Count < 5) && i == (members.Count - 1))
                    || (members.Count >= 5 && (i == 4 || i == 9)))
                {
                    _httpRequests.Add(new HttpMessage
                    {
                        requestType = RequestType.GetDivineStratzGraphQLData,
                        players = _requestPlayers.ToList(),
                        startDateTime = DateTime.UtcNow.AddMonths(-1)
                    });
                    _requestPlayers.Clear();
                }
            }
        }

        private static uint SteamId64to32(ulong steamId64)
        {
            return (uint)(steamId64 & 0xFFFFFFFF);
        }

        internal void Dispose()
        {
        }
    }
}
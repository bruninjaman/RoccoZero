using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Network.GC;

using Overwolf.Requests.Http.Models.Divine;

using System;
using System.Collections.Generic;

namespace Overwolf.Data
{
    internal sealed class Data
    {
        internal class PlayerData
        {
            internal bool partyWithNextPlayer { get; set; }
            internal uint id { get; set; }
            internal HeroId heroId { get; set; }
            internal string name { get; set; }
            internal LaneSelectonFlags laneSelectonFlags { get; set; }
            internal int rankTier { get; set; }
            internal ulong partyId { get; set; }
            internal Dictionary<int, HeroPerformance> mostPlayed { get; set; } = new Dictionary<int, HeroPerformance>();
            internal Dictionary<int, Match> recentMatches { get; set; } = new Dictionary<int, Match>();
            internal bool isAnonymous { get; set; } = true;
            internal uint matchCount { get; set; }
            internal uint winPercent { get; set; }
        }

        internal class Match
        {
            internal ulong matchId { get; set; }
            internal uint matchTimestamp { get; set; }
            internal int performanceRating { get; set; }
            internal HeroId heroId { get; set; }
            internal bool wonMatch { get; set; }
            internal uint kills { get; set; }
            internal uint deaths { get; set; }
            internal uint assists { get; set; }
            internal uint laneSelectionFlags { get; set; }
            internal uint XPPerMin { get; set; }
            internal uint goldPerMin { get; set; }
            internal uint denies { get; set; }
            internal uint lastHits { get; set; }
            internal uint duration { get; set; }
        }

        internal class ProtoMessage
        {
            internal GCMessageId msgId { get; set; }
            internal uint accountId { get; set; }
            internal ulong matchId { get; set; }
            internal ulong jobId { get; set; }
            internal bool sent { get; set; }
        }

        internal class HttpMessage
        {
            internal ulong matchId { get; set; }
            internal RequestType requestType { get; set; }
            internal uint accountId { get; set; }
            internal int playerNumber { get; set; }
            internal int matchNumber { get; set; }
            internal DateTime startDateTime { get; set; }
            internal bool sent { get; set; }
        }

        internal enum RequestType
        {
            GetDivineStratzData = 0,
            GetDivineMatchDetails = 1,
        }

        internal enum LaneSelectonFlags
        {
            Unknown = 0,
            safelane = 1, //1 << 0
            offlane = 2, //1 << 1
            midlane = 4, //1 << 2
            softsupport = 8, //1 << 3
            hardsupport = 16, //1 << 4
        }

        internal enum LobbyType
        {
            INVALID = -1,
            CASUAL_MATCH = 0,
            PRACTICE = 1,
            COOP_BOT_MATCH = 4,
            LEGACY_TEAM_MATCH = 5,
            LEGACY_SOLO_QUEUE_MATCH = 6,
            COMPETITIVE_MATCH = 7,
            CASUAL_1V1_MATCH = 8,
            WEEKEND_TOURNEY = 9,
            LOCAL_BOT_MATCH = 10,
            SPECTATOR = 11,
            EVENT_MATCH = 12,
            GAUNTLET = 13,
            NEW_PLAYER_POOL = 14,
        }

        internal enum Engine
        {
            SOURCE_1 = 0,
            SOURCE_2 = 1,
        }
    }
}
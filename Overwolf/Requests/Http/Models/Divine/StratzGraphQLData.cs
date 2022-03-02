using Divine.Entity.Entities.Units.Heroes.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overwolf.Requests.Http.Models.Divine.GraphQL
{
    public class GameVersion
    {
        public int? id { get; set; }
        public string name { get; set; }
        public int? asOfDateTime { get; set; }
    }

    public class Constants
    {
        public List<GameVersion> gameVersions { get; set; }
    }

    public class Rank
    {
        public int? rank { get; set; }
    }

    public class SteamAccount
    {
        public int? id { get; set; }
        public string name { get; set; }
        public int? seasonRank { get; set; }
        public string countryCode { get; set; }
        public bool? isAnonymous { get; set; }
    }

    public class Hero
    {
        public string displayName { get; set; }
    }

    public class HeroesPerformance
    {
        public int? experiencePerMinute { get; set; }
        public int? goldPerMinute { get; set; }
        public HeroId heroId { get; set; }
        public int? imp { get; set; }
        public double? kDA { get; set; }
        public int? lastPlayedDateTime { get; set; }
        public int? matchCount { get; set; }
        public int? winCount { get; set; }
        public Hero hero { get; set; }
    }

    public class PickBan
    {
        public HeroId? heroId { get; set; }
        public int? order { get; set; }
        public bool? isPick { get; set; }
    }

    public class Stats
    {
        public List<PickBan> pickBans { get; set; }
    }

    public class MatchPlayer
    {
        public int? imp { get; set; }
        public int? experiencePerMinute { get; set; }
        public int? goldPerMinute { get; set; }
        public HeroId? heroId { get; set; }
        public bool? isRadiant { get; set; }
        public string lane { get; set; }
        public string leaverStatus { get; set; }
        public int? kills { get; set; }
        public int? deaths { get; set; }
        public int? assists { get; set; }
        public string roleBasic { get; set; }
        public int? steamAccountId { get; set; }
        public ulong? partyId { get; set; }
        public int? numLastHits { get; set; }
        public int? numDenies { get; set; }
        public Hero hero { get; set; }
    }

    public class Player
    {
        public int? winCount { get; set; }
        public int? matchCount { get; set; }
        public List<Rank> ranks { get; set; }
        public SteamAccount steamAccount { get; set; }
        public List<HeroesPerformance> heroesPerformance { get; set; }
        public List<Match> matches { get; set; }
        public int? imp { get; set; }
        public int? behaviorScore { get; set; }
    }

    public class Match
    {
        public bool? didRadiantWin { get; set; }
        public int? endDateTime { get; set; }
        public ulong? id { get; set; }
        public int? rank { get; set; }
        public string lobbyType { get; set; }
        public string gameMode { get; set; }
        public int? durationSeconds { get; set; }
        public Stats stats { get; set; }
        public List<MatchPlayer> players { get; set; }
    }

    public class StratzGraphQLData
    {
        public Constants constants { get; set; }
        public List<Player> players { get; set; }
    }

}

using Divine.Entity.Entities.Units.Heroes.Components;

using System;
using System.Collections.Generic;

namespace Overwolf.Requests.Http.Models.Divine
{
    public class Identity
    {
        public string captainJackIdentityId { get; set; }
        public string name { get; set; }
        public int feedLevel { get; set; }
        public int emailLevel { get; set; }
        public bool dailyEmail { get; set; }
        public bool weeklyEmail { get; set; }
        public bool monthlyEmail { get; set; }
        public int proCircuitFeedLevel { get; set; }
        public int proCircuitEmailLevel { get; set; }
        public int themeType { get; set; }
        public int languageId { get; set; }
        public bool isEmailValidated { get; set; }
        public int emailHour { get; set; }
        public long lastDailyEmail { get; set; }
        public long lastWeeklyEmail { get; set; }
        public long lastMonthlyEmail { get; set; }
        public long lastLeagueDailyEmail { get; set; }
        public long lastTeamDailyEmail { get; set; }
        public long lastProCircuitDailyEmail { get; set; }
        public string unsubscribeCode { get; set; }
    }

    public class SteamAccount
    {
        public int id { get; set; }
        public int steamId { get; set; }
        public DateTime lastActiveTime { get; set; }
        public string profileUri { get; set; }
        public int cityId { get; set; }
        public int communityVisibleState { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public long primaryClanId { get; set; }
        public int soloRank { get; set; }
        public int partyRank { get; set; }
        public bool isDotaPlusSubscriber { get; set; }
        public int dotaPlusOriginalStartDate { get; set; }
        public bool isAnonymous { get; set; }
        public bool isStratzAnonymous { get; set; }
        public int seasonRank { get; set; }
        public int seasonLeaderboardRank { get; set; }
        public int seasonLeaderboardDivisionId { get; set; }
        public int smurfFlag { get; set; }
        public int smurfCheckDate { get; set; }
        public int lastMatchDateTime { get; set; }
        public int lastMatchRegionId { get; set; }
    }

    public class BattlePass
    {
        public int eventId { get; set; }
        public int level { get; set; }
    }

    public class Rank
    {
        public int seasonRankId { get; set; }
        public DateTime asOfDateTime { get; set; }
        public bool isCore { get; set; }
        public int rank { get; set; }
    }

    public class Name
    {
        public string name { get; set; }
        public int lastSeenDateTime { get; set; }
    }

    public class MainInfo
    {
        public Identity identity { get; set; }
        public SteamAccount steamAccount { get; set; }
        public List<BattlePass> battlePass { get; set; }
        public int date { get; set; }
        public int lastRegionId { get; set; }
        public List<Rank> ranks { get; set; }
        public List<string> languageCode { get; set; }
        public int firstMatchDate { get; set; }
        public int matchCount { get; set; }
        public int winCount { get; set; }
        public List<Name> names { get; set; }
        public int behaviorScore { get; set; }
        public int steamAccountId { get; set; }
        public bool isFollowed { get; set; }
    }

    public class Matches
    {
        public int matchCount { get; set; }
        public int win { get; set; }
        public int imp { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class IsStatsMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class RankMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class LobbyMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class GameModeMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class FactionMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int imp { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class RegionMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int imp { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class LaneMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class RoleMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class PartyMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class ImpMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class DurationMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class HeroAttributeMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class DayOfWeekMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class TimeOfDayMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class WeekEndMatch
    {
        public int id { get; set; }
        public int matchCount { get; set; }
        public int win { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public int date { get; set; }
    }

    public class AllTime
    {
        public Matches matches { get; set; }
        public List<IsStatsMatch> isStatsMatches { get; set; }
        public List<RankMatch> rankMatches { get; set; }
        public List<LobbyMatch> lobbyMatches { get; set; }
        public List<GameModeMatch> gameModeMatches { get; set; }
        public List<FactionMatch> factionMatches { get; set; }
        public List<RegionMatch> regionMatches { get; set; }
        public List<LaneMatch> laneMatches { get; set; }
        public List<RoleMatch> roleMatches { get; set; }
        public List<PartyMatch> partyMatches { get; set; }
        public List<ImpMatch> impMatches { get; set; }
        public List<DurationMatch> durationMatches { get; set; }
        public List<HeroAttributeMatch> heroAttributeMatches { get; set; }
        public List<DayOfWeekMatch> dayOfWeekMatches { get; set; }
        public List<TimeOfDayMatch> timeOfDayMatches { get; set; }
        public List<WeekEndMatch> weekEndMatches { get; set; }
    }

    public class SixMonths
    {
        public Matches matches { get; set; }
        public List<IsStatsMatch> isStatsMatches { get; set; }
        public List<RankMatch> rankMatches { get; set; }
        public List<LobbyMatch> lobbyMatches { get; set; }
        public List<GameModeMatch> gameModeMatches { get; set; }
        public List<FactionMatch> factionMatches { get; set; }
        public List<RegionMatch> regionMatches { get; set; }
        public List<LaneMatch> laneMatches { get; set; }
        public List<RoleMatch> roleMatches { get; set; }
        public List<PartyMatch> partyMatches { get; set; }
        public List<ImpMatch> impMatches { get; set; }
        public List<DurationMatch> durationMatches { get; set; }
        public List<HeroAttributeMatch> heroAttributeMatches { get; set; }
        public List<DayOfWeekMatch> dayOfWeekMatches { get; set; }
        public List<TimeOfDayMatch> timeOfDayMatches { get; set; }
        public List<WeekEndMatch> weekEndMatches { get; set; }
    }

    public class OneMonth
    {
        public Matches matches { get; set; }
        public List<IsStatsMatch> isStatsMatches { get; set; }
        public List<RankMatch> rankMatches { get; set; }
        public List<LobbyMatch> lobbyMatches { get; set; }
        public List<GameModeMatch> gameModeMatches { get; set; }
        public List<FactionMatch> factionMatches { get; set; }
        public List<RegionMatch> regionMatches { get; set; }
        public List<LaneMatch> laneMatches { get; set; }
        public List<RoleMatch> roleMatches { get; set; }
        public List<PartyMatch> partyMatches { get; set; }
        public List<ImpMatch> impMatches { get; set; }
        public List<DurationMatch> durationMatches { get; set; }
        public List<HeroAttributeMatch> heroAttributeMatches { get; set; }
        public List<DayOfWeekMatch> dayOfWeekMatches { get; set; }
        public List<TimeOfDayMatch> timeOfDayMatches { get; set; }
        public List<WeekEndMatch> weekEndMatches { get; set; }
    }

    public class PlayerSummary
    {
        public AllTime allTime { get; set; }
        public SixMonths sixMonths { get; set; }
        public OneMonth oneMonth { get; set; }
    }

    public class LaneScore
    {
        public int id { get; set; }
        public double score { get; set; }
        public int matchCount { get; set; }
        public int winCount { get; set; }
        public int imp { get; set; }
    }

    public class RoleScore
    {
        public int id { get; set; }
        public double score { get; set; }
        public int matchCount { get; set; }
        public int winCount { get; set; }
        public int imp { get; set; }
    }

    public class HeroPerformance
    {
        public HeroId heroId { get; set; }
        public int winCount { get; set; }
        public double kda { get; set; }
        public int duration { get; set; }
        public int imp { get; set; }
        public int best { get; set; }
        public int matchCount { get; set; }
        public int goldPerMinute { get; set; }
        public int experiencePerMinute { get; set; }
        public List<LaneScore> laneScore { get; set; }
        public List<RoleScore> roleScore { get; set; }
        public int lastPlayed { get; set; }
    }

    public class StratzData
    {
        public MainInfo mainInfo { get; set; }
        public PlayerSummary playerSummary { get; set; }
        public List<HeroPerformance> heroPerformance { get; set; }
    }
}

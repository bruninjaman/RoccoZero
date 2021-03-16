using System.Collections.Generic;

using Divine;

using Ensage;
using Ensage.SDK.Helpers;
using SharpDX;

namespace TinkerCrappahilationPaid
{
    public class LaneHelper
    {
        public readonly TinkerCrappahilationPaid Main;
        public Map Map;

        public LaneHelper(TinkerCrappahilationPaid main)
        {
            Main = main;
            Map = new Map();
            var isRadiant = EntityManager.LocalHero.Team == Team.Radiant;
            TopPath = isRadiant ? Map.RadiantTopRoute : Map.DireTopRoute;
            MidPath = isRadiant ? Map.RadiantMiddleRoute : Map.DireMiddleRoute;
            BotPath = isRadiant ? Map.RadiantBottomRoute : Map.DireBottomRoute;
            LaneCache = new Dictionary<Unit, List<Vector3>>();
        }

        public List<Vector3> BotPath { get; set; }
        public List<Vector3> MidPath { get; set; }
        public List<Vector3> TopPath { get; set; }
        public Dictionary<Unit, List<Vector3>> LaneCache;

        public List<Vector3> GetPathCache(Unit hero)
        {
            if (!LaneCache.ContainsKey(hero))
            {
                LaneCache.Add(hero, GetPath(hero));
                UpdateManager.BeginInvoke(150, () =>
                {
                    LaneCache.Remove(hero);
                });
            }
            return LaneCache[hero];
        }

        public List<Vector3> GetPath(Unit hero)
        {
            var currentLane = GetLane(hero);
            switch (currentLane)
            {
                case MapArea.Top:
                    return TopPath;
                case MapArea.Middle:
                    return MidPath;
                case MapArea.Bottom:
                    return BotPath;
                case MapArea.DireTopJungle:
                    return TopPath;
                case MapArea.RadiantBottomJungle:
                    return BotPath;
                case MapArea.RadiantTopJungle:
                    return TopPath;
                case MapArea.DireBottomJungle:
                    return BotPath;
                default:
                    return MidPath;
            }
        }
        public List<Vector3> GetPath(MapArea currentLane)
        {
            switch (currentLane)
            {
                case MapArea.Top:
                    return TopPath;
                case MapArea.Middle:
                    return MidPath;
                case MapArea.Bottom:
                    return BotPath;
                case MapArea.DireTopJungle:
                    return TopPath;
                case MapArea.RadiantBottomJungle:
                    return BotPath;
                case MapArea.RadiantTopJungle:
                    return TopPath;
                case MapArea.DireBottomJungle:
                    return BotPath;
                default:
                    return MidPath;
            }
        }

        public MapArea GetLane(Unit hero)
        {
            var lane = GetLane(hero.Position);
            switch (lane)
            {
                case MapArea.Top:
                    return MapArea.Top;
                case MapArea.Middle:
                    return MapArea.Middle;
                case MapArea.Bottom:
                    return MapArea.Bottom;
                case MapArea.DireTopJungle:
                    return MapArea.Top;
                case MapArea.RadiantBottomJungle:
                    return MapArea.Bottom;
                case MapArea.RadiantTopJungle:
                    return MapArea.Top;
                case MapArea.DireBottomJungle:
                    return MapArea.Bottom;
                default:
                    return MapArea.Middle;
            }
        }

        public MapArea GetLane(Vector3 pos)
        {
            if (Map.Top.IsInside(pos))
            {
                return MapArea.Top;
            }
            if (Map.Middle.IsInside(pos))
            {
                return MapArea.Middle;
            }
            if (Map.Bottom.IsInside(pos))
            {
                return MapArea.Bottom;
            }
            if (Map.River.IsInside(pos))
            {
                return MapArea.River;
            }
            if (Map.RadiantBase.IsInside(pos))
            {
                return MapArea.RadiantBase;
            }
            if (Map.DireBase.IsInside(pos))
            {
                return MapArea.DireBase;
            }
            if (Map.Roshan.IsInside(pos))
            {
                return MapArea.RoshanPit;
            }
            if (Map.DireBottomJungle.IsInside(pos))
            {
                return MapArea.DireBottomJungle;
            }
            if (Map.DireTopJungle.IsInside(pos))
            {
                return MapArea.DireTopJungle;
            }
            if (Map.RadiantBottomJungle.IsInside(pos))
            {
                return MapArea.RadiantBottomJungle;
            }
            if (Map.RadiantTopJungle.IsInside(pos))
            {
                return MapArea.RadiantTopJungle;
            }

            return MapArea.Unknown;
        }
    }
}
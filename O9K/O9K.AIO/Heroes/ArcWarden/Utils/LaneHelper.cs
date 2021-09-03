namespace O9K.AIO.Heroes.ArcWarden.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Units;
    using Core.Managers.Entity;

    using Divine.Entity.Entities.Components;
    using Divine.Entity.Entities.Units;
    using Divine.Extensions;
    using Divine.Numerics;
    using Divine.Update;

    public class LaneHelper
    {
        private static readonly Dictionary<Vector3, Lane> LaneDictionary = new()
        {
            { new Vector3(-6080, 5805, 384), Lane.TOP },
            { new Vector3(-6600, -3000, 384), Lane.TOP },
            { new Vector3(2700, 5600, 384), Lane.TOP },

            { new Vector3(5807, -5785, 384), Lane.BOT },
            { new Vector3(-3200, -6200, 384), Lane.BOT },
            { new Vector3(6200, 2200, 384), Lane.BOT },

            { new Vector3(-600, -300, 384), Lane.MID },
            { new Vector3(3600, 3200, 384), Lane.MID },
            { new Vector3(-4400, -3900, 384), Lane.MID },
        };

        public Dictionary<Unit, List<Vector3>> LaneCache;

        public LaneHelper()
        {
            bool isRadiant = EntityManager9.Owner.Team == Team.Radiant;
            this.TopPath = isRadiant ? Map.radiantTopPath : Map.direTopPath;
            this.MidPath = isRadiant ? Map.radiantMidPath : Map.direMidPath;
            this.BotPath = isRadiant ? Map.radiantBotPath : Map.direBotPath;
            this.LaneCache = new Dictionary<Unit, List<Vector3>>();
        }

        public List<Vector3> BotPath { get; set; }

        public List<Vector3> MidPath { get; set; }

        public List<Vector3> TopPath { get; set; }

        public List<Vector3> GetPathCache(Unit9 hero)
        {
            if (!this.LaneCache.ContainsKey(hero))
            {
                this.LaneCache.Add(hero, GetPath(hero));

                UpdateManager.BeginInvoke(150, () =>
                                               {
                                                   this.LaneCache.Remove(hero);
                                               });
            }

            return this.LaneCache[hero];
        }

        public List<Vector3> GetPath(Unit9 hero)
        {
            var currentLane = GetCurrentLane(hero);

            switch (currentLane)
            {
                case Lane.TOP:
                    return this.TopPath;

                case Lane.MID:
                    return this.MidPath;

                case Lane.BOT:
                    return this.BotPath;

                default:
                    return this.MidPath;
            }
        }

        public List<Vector3> GetPath(Lane currentLane)
        {
            switch (currentLane)
            {
                case Lane.TOP:
                    return this.TopPath;

                case Lane.MID:
                    return this.MidPath;

                case Lane.BOT:
                    return this.BotPath;

                default:
                    return this.MidPath;
            }
        }

        public Lane GetCurrentLane(Unit9 hero)
        {
            return GetCurrentLane(hero.Position);
        }

        public Lane GetCurrentLane(Vector3 pos)
        {
            return LaneDictionary.OrderBy(x => x.Key.Distance2D(pos)).First().Value;
        }

        public Vector3 GetClosestAttackPoint(Unit9 owner, Lane currentLane)
        {
            List<Vector3> list;

            switch (currentLane)
            {
                case Lane.TOP:
                    list = this.TopPath;

                    break;

                case Lane.MID:
                    list = this.MidPath;

                    break;

                case Lane.BOT:
                    list = this.BotPath;

                    break;

                default:
                    list = this.MidPath;

                    break;
            }

            int result = 0;

            for (int index = 0; index < list.Count; index++)
            {
                var vector3 = list[index];

                if (owner.Distance(vector3) < owner.Distance(list[result]))
                {
                    result = index;
                }
            }

            if (EntityManager9.AllyFountain.Distance(list[result]) <=
                EntityManager9.AllyFountain.Distance(owner.Position) || owner.Distance(list[result]) < 100)
            {
                result += 1;
            }

            if (result >= list.Count)
            {
                return list[list.Count - 1];
            }

            return list[result];
        }
    }
}
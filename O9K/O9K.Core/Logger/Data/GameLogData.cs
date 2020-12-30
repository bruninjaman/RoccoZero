namespace O9K.Core.Logger.Data
{
    using System;

    using Core.Data;

    using Divine;

    [Serializable]
    internal sealed class GameLogData
    {
        public GameLogData()
        {
            try
            {
                var hero = EntityManager.LocalHero;

                this.Hero = hero.Name;
                this.Team = hero.Team.ToString();
                this.Map = GameManager.ShortLevelName;
                this.Mode = GameManager.GameMode.ToString();
                this.State = GameManager.GameState.ToString();
                this.Time = GameData.DisplayTime;
            }
            catch
            {
                // ignored
            }
        }

        public string Hero { get; }

        public string Map { get; }

        public string Mode { get; }

        public string State { get; }

        public string Team { get; }

        public string Time { get; }
    }
}
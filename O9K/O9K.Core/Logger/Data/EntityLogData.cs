namespace O9K.Core.Logger.Data
{
    using System;

    using Divine.Entity.Entities;
    using Divine.Entity.Entities.Players;

    [Serializable]
    internal sealed class EntityLogData
    {
        public EntityLogData(Entity entity)
        {
            try
            {
                this.Name = entity.Name;
                this.Team = entity.Team.ToString();

                var owner = entity.Owner;
                if (entity.Owner?.IsValid == true)
                {
                    this.Owner = owner is Player ? "Player" : owner.Name;
                }
                else
                {
                    this.Owner = "null";
                }
            }
            catch
            {
                // ignored
            }
        }

        public string Name { get; }

        public string Owner { get; }

        public string Team { get; }
    }
}
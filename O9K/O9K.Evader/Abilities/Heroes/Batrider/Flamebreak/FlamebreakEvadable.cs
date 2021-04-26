namespace O9K.Evader.Abilities.Heroes.Batrider.Flamebreak
{
    using System;

    using Base.Evadable;
    using Base.Evadable.Components;

    using Core.Entities.Abilities.Base;
    using Core.Logger;

    using Divine;
    using Divine.SDK.Extensions;

    using Metadata;

    using Pathfinder.Obstacles.Abilities.AreaOfEffect;

    internal sealed class FlamebreakEvadable : AreaOfEffectEvadable, IParticle
    {
        public FlamebreakEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
            : base(ability, pathfinder, menu)
        {
            this.Counters.Add(Abilities.SleightOfFist);
            this.Counters.Add(Abilities.BallLightning);
            this.Counters.UnionWith(Abilities.Shield);
            this.Counters.UnionWith(Abilities.MagicShield);
        }

        public void AddParticle(Particle particle, string name)
        {
            var time = GameManager.RawGameTime - (GameManager.Ping / 2000);

            UpdateManager.BeginInvoke(
                () =>
                    {
                        try
                        {
                            if (!particle.IsValid)
                            {
                                return;
                            }

                            var startPosition = particle.GetControlPoint(0);
                            var endPosition = particle.GetControlPoint(5);

                            var obstacle = new AreaOfEffectObstacle(this, endPosition)
                            {
                                EndObstacleTime = time + (startPosition.Distance2D(endPosition) / this.ActiveAbility.Speed)
                            };

                            this.Pathfinder.AddObstacle(obstacle);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                    });
        }

        public override void PhaseCancel()
        {
        }

        public override void PhaseStart()
        {
        }
    }
}
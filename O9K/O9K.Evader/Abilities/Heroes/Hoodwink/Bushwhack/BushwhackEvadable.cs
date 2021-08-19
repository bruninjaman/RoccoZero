namespace O9K.Evader.Abilities.Heroes.Hoodwink.Bushwhack
{
    using System;
    using System.Linq;

    using Base.Evadable;
    using Base.Evadable.Components;

    using Core.Entities.Abilities.Base;
    using Core.Logger;

    using Divine.Entity;
    using Divine.Entity.Entities.Trees;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Particle.Particles;
    using Divine.Update;

    using Metadata;

    using Pathfinder.Obstacles.Abilities.AreaOfEffect;

    internal sealed class BushwhackEvadable :  AreaOfEffectEvadable, IParticle
    {
        public BushwhackEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
            : base(ability, pathfinder, menu)
        {
            this.Counters.Add(Abilities.SleightOfFist);
            this.Counters.Add(Abilities.BallLightning);
            this.Counters.Add(Abilities.Spoink);
            this.Counters.Add(Abilities.MantaStyle);
            this.Counters.Add(Abilities.AttributeShift);
            this.Counters.UnionWith(Abilities.StrongShield);
            this.Counters.UnionWith(Abilities.MagicShield);
            this.Counters.UnionWith(Abilities.Heal);
            this.Counters.Add(Abilities.Armlet);
            this.Counters.UnionWith(Abilities.Suicide);
            this.Counters.Add(Abilities.BladeMail);
            this.Counters.UnionWith(Abilities.VsDisableProjectile);

            this.Counters.Remove(Abilities.DarkPact);
            this.Counters.Remove(Abilities.ShadowDance);
            this.Counters.Remove(Abilities.ShadowRealm);
            this.Counters.ExceptWith(Abilities.MagicImmunity);
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
                        var endPosition = particle.GetControlPoint(1);

                        if (!EntityManager.GetEntities<Tree>().Any(x => endPosition.Distance2D(x.Position) < this.Ability.Radius))
                        {
                            return;
                        }

                        var obstacle = new AreaOfEffectObstacle(this, endPosition)
                        {
                            EndCastTime = time,
                            EndObstacleTime = (time + (startPosition.Distance2D(endPosition) / this.ActiveAbility.Speed)) - 0.1f
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
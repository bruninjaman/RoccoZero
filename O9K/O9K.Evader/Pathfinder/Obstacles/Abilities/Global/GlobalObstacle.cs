namespace O9K.Evader.Pathfinder.Obstacles.Abilities.Global
{
    using Core.Entities.Units;

    using Divine;

    using O9K.Evader.Abilities.Base.Evadable;

    internal class GlobalObstacle : AbilityObstacle
    {
        public GlobalObstacle(EvadableAbility ability)
            : base(ability)
        {
        }

        public override void Draw()
        {
        }

        public override float GetDisableTime(Unit9 enemy)
        {
            return this.EndCastTime - GameManager.RawGameTime;
        }

        public override float GetEvadeTime(Unit9 ally, bool blink)
        {
            return this.EndObstacleTime - GameManager.RawGameTime;
        }

        public override bool IsIntersecting(Unit9 unit, bool checkPrediction)
        {
            return true;
        }
    }
}
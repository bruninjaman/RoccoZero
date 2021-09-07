namespace O9K.Evader.Abilities.Heroes.Bristleback.QuillSpray;

using Base.Evadable;

using Core.Entities.Units;

using Divine.Numerics;

using Pathfinder.Obstacles.Abilities.AreaOfEffect;

internal class QuillSprayObstacle : AreaOfEffectSpeedObstacle
{
    public QuillSprayObstacle(EvadableAbility ability, Vector3 position, float damageRadius)
        : base(ability, position, damageRadius)
    {
    }

    public override bool IsIntersecting(Unit9 unit, bool checkPrediction)
    {
        if (!base.IsIntersecting(unit, checkPrediction))
        {
            return false;
        }

        if (unit.GetModifierStacks("modifier_bristleback_quill_spray") < 5)
        {
            return false;
        }

        return true;
    }
}
namespace O9K.AIO.Heroes.ShadowFiend.Abilities;

using System;

using AIO.Abilities;

using Core.Entities.Abilities.Base;
using Core.Extensions;
using Core.Geometry;

using Divine.Extensions;

using Modes.Combo;

using TargetManager;

internal class ShadowRaze : NukeAbility
{
    public ShadowRaze(ActiveAbility ability)
        : base(ability)
    {
    }

    public override bool CanHit(TargetManager targetManager, IComboModeMenu comboMenu)
    {
        var target = targetManager.Target;
        var predictionInput = this.Ability.GetPredictionInput(target);
        var output = this.Ability.GetPredictionOutput(predictionInput);
        var predictedPosition = output.TargetPosition;
        var currentPosition = target.Position;
        var circle = new Polygon9.Circle(this.Owner.Position.Extend2D(currentPosition, this.Ability.Range),
                                         this.Ability.Radius * 0.2f);

        if (Divine.Helpers.MultiSleeper<string>.Sleeping("FailSafe.Razes") && circle.IsOutside(currentPosition.ToVector2()))
        {
            Console.WriteLine("FailSafe.Razes");
            return false;
        }
        Divine.Helpers.MultiSleeper<string>.Reset("FailSafe.Razes");

        return base.CanHit(targetManager, comboMenu);
    }
}
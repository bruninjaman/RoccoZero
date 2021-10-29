namespace O9K.AIO.Heroes.Marci.Abilities;

using System;

using AIO.Abilities;

using Core.Entities.Abilities.Base;
using Core.Extensions;
using Core.Helpers;

using Divine.Numerics;

using TargetManager;

using BaseRebound = Core.Entities.Abilities.Heroes.Marci.Rebound;

internal class ReboundBlink : BlinkAbility
{
    private readonly BaseRebound baseRebound;

    public ReboundBlink(ActiveAbility ability)
        : base(ability)
    {
        this.baseRebound = (BaseRebound)ability;
    }

    //public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, Vector3 toPosition)
    //{
    //    var position = this.Owner.Position.Extend2D(toPosition, Math.Min(this.Ability.CastRange - 25, this.Owner.Distance(toPosition)));

    //    var target = targetManager.Target;

    //    if (target != null)
    //    {
    //        if (!this.baseRebound.UseAbility(position, target.GetPredictedPosition(this.Ability.GetHitTime(target.Position))))
    //        {
    //            return false;
    //        }
    //    }
    //    else
    //    {
    //        if (!this.baseRebound.UseAbility(position, position))
    //        {
    //            return false;
    //        }
    //    }

    //    var delay = this.Ability.GetCastDelay(position);
    //    comboSleeper.Sleep(delay);
    //    this.Sleeper.Sleep(delay + 0.5f);
    //    this.OrbwalkSleeper.Sleep(delay);

    //    return true;
    //}
}
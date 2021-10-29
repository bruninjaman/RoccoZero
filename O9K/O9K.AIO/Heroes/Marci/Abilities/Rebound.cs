namespace O9K.AIO.Heroes.Marci.Abilities;

using AIO.Abilities;

using Core.Entities.Abilities.Base;
using Core.Entities.Abilities.Base.Types;
using Core.Entities.Units;
using Core.Helpers;

using TargetManager;

internal class Rebound : NukeAbility
{
    private Unit9 target;

    public Rebound(ActiveAbility ability)
        : base(ability)
    {
    }

    //public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
    //{
    //    if (this.target == null)
    //    {
    //        this.target = targetManager.Target;
    //    }

    //    if (!this.Ability.UseAbility(this.target))
    //    {
    //        return false;
    //    }

    //    var delay = this.Ability.GetCastDelay(targetManager.Target);
    //    if (this.Ability is IDisable disable)
    //    {
    //        targetManager.Target.SetExpectedUnitState(disable.AppliesUnitState, this.Ability.GetHitTime(targetManager.Target));
    //    }

    //    comboSleeper.Sleep(delay);
    //    this.Sleeper.Sleep(delay + 0.5f);
    //    this.OrbwalkSleeper.Sleep(delay);
    //    this.target = null;
    //    return true;
    //}
}
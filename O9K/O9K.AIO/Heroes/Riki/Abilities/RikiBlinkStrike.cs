namespace O9K.AIO.Heroes.Riki.Abilities;

using AIO.Abilities;

using Core.Entities.Abilities.Base;
using Core.Helpers;

using TargetManager;

internal class RikiBlinkStrike : BlinkAbility
{
    public RikiBlinkStrike(ActiveAbility ability)
        : base(ability)
    {
    }

    public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
    {
        var target = targetManager.Target;
        var attackRange = this.Owner.GetAttackRange(target);
        var distance = target.Distance(this.Owner);

        if (distance <= attackRange + 100)
        {
            return false;
        }

        if (distance <= attackRange + 250 && this.Owner.Speed > targetManager.Target.Speed + 50)
        {
            return false;
        }

        if (this.Ability.UseAbility(target))
        {
            return  false;
        }

        var delay = this.Ability.GetCastDelay(target);
        comboSleeper.Sleep(delay);
        this.Sleeper.Sleep(delay + 0.5f);
        this.OrbwalkSleeper.Sleep(delay);

        return true;
    }
}
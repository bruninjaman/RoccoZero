using O9K.AIO.Abilities;
using O9K.AIO.Modes.Combo;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Extensions;
using O9K.Core.Helpers;

namespace O9K.AIO.Heroes.Kunkka.Abilities;
using FailSafe;
internal class TidalWave : DisableAbility
{
    public TidalWave(ActiveAbility baseAbility) : base(baseAbility)
    {
    }

    public FailSafe FailSafe { get; set; }

    public override bool CanHit(TargetManager.TargetManager targetManager, IComboModeMenu comboMenu)
    {
        var target = targetManager.Target;
        if (!target.IsMagicImmune && target.Distance(Owner) <= 900)
        {
            return true;
        }
        return base.CanHit(targetManager, comboMenu);
    }

    public override bool UseAbility(TargetManager.TargetManager targetManager, Sleeper comboSleeper, bool aoe)
    {
        var target = targetManager.Target;
        if (!target.IsMagicImmune && target.Distance(Owner) <= 900)
        {
            var targetPos = Owner.Position.Extend2D(targetManager.Target.Position, -100);
            if (!this.Ability.UseAbility(targetPos))
            {
                return false;
            }

            var delay = this.Ability.GetCastDelay(targetManager.Target);

            targetManager.Target.SetExpectedUnitState(this.Disable.AppliesUnitState, delay);
            comboSleeper.Sleep(delay);
            this.OrbwalkSleeper.Sleep(delay);
            this.Sleeper.Sleep(delay);
            this.FailSafe.Sleeper.Sleep(delay);
            return true;
        }
        return base.UseAbility(targetManager, comboSleeper, aoe);
    }
}
﻿namespace O9K.AIO.Heroes.AncientApparition.Abilities;

using AIO.Abilities;

using Core.Entities.Abilities.Base;
using Core.Helpers;

using TargetManager;

internal class ChillingTouch : TargetableAbility
{
    public ChillingTouch(ActiveAbility ability)
        : base(ability)
    {
    }

    public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
    {
        if (!this.Ability.UseAbility(targetManager.Target))
        {
            return false;
        }

        var delay = this.Ability.GetCastDelay(targetManager.Target);
        comboSleeper.Sleep(delay);
        this.Sleeper.Sleep(delay + 0.5f);
        this.OrbwalkSleeper.Sleep(delay);
        return true;
    }
}
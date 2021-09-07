﻿namespace O9K.Core.Entities.Abilities.Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units.Components;

public abstract class PassiveAbility : Ability9
{
    protected PassiveAbility(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public override bool CanBeCasted(bool checkChanneling = true)
    {
        if (!this.IsReady)
        {
            return false;
        }

        return (this.Owner.UnitState & UnitState.PassivesDisabled) == 0;
    }
}
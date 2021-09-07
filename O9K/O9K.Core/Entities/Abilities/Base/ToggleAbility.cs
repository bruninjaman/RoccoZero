﻿namespace O9K.Core.Entities.Abilities.Base;

using Components;

using Divine.Entity.Entities.Abilities;

using Entities.Units;

public abstract class ToggleAbility : ActiveAbility, IToggleable
{
    protected ToggleAbility(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public virtual bool Enabled
    {
        get
        {
            return this.BaseAbility.IsToggled;
        }
        set
        {
            var result = false;
            if (value)
            {
                if (!this.Enabled)
                {
                    result = this.BaseAbility.CastToggle();
                }
            }
            else
            {
                if (this.Enabled)
                {
                    result = this.BaseAbility.CastToggle();
                }
            }

            if (result)
            {
                this.ActionSleeper.Sleep(0.1f);
            }
        }
    }

    public override bool UseAbility(Unit9 target, bool queue = false, bool bypass = false)
    {
        return this.UseAbility(queue, bypass);
    }

    public override bool UseAbility(bool queue = false, bool bypass = false)
    {
        var result = this.BaseAbility.CastToggle(queue, bypass);
        if (result)
        {
            this.ActionSleeper.Sleep(0.1f);
        }

        return result;
    }
}
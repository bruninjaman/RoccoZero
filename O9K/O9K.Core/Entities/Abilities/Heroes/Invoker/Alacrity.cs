﻿namespace O9K.Core.Entities.Abilities.Heroes.Invoker;

using System.Collections.Generic;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Entities.Units;

using Helpers;

using Metadata;

[AbilityId(AbilityId.invoker_alacrity)]
public class Alacrity : RangedAbility, IInvokableAbility, IBuff
{
    private readonly InvokeHelper<Alacrity> invokeHelper;

    public Alacrity(Ability baseAbility)
        : base(baseAbility)
    {
        this.invokeHelper = new InvokeHelper<Alacrity>(this);
    }

    public string BuffModifierName { get; } = "modifier_invoker_alacrity";

    public bool BuffsAlly { get; } = true;

    public bool BuffsOwner { get; } = true;

    public bool CanBeInvoked
    {
        get
        {
            if (this.IsInvoked)
            {
                return true;
            }

            return this.invokeHelper.CanInvoke(false);
        }
    }

    public bool IsInvoked
    {
        get
        {
            return this.invokeHelper.IsInvoked;
        }
    }

    public override bool IsUsable
    {
        get
        {
            if (!this.IsAvailable)
            {
                return false;
            }

            return true;
        }
    }

    public AbilityId[] RequiredOrbs { get; } = { AbilityId.invoker_wex, AbilityId.invoker_wex, AbilityId.invoker_exort };
    public AbilitySlot GetAbilitySlot => invokeHelper.GetAbilitySlot;

    public override bool CanBeCasted(bool checkChanneling = true)
    {
        return base.CanBeCasted(checkChanneling) && this.invokeHelper.CanInvoke(!this.IsInvoked);
    }

    public bool Invoke(List<AbilityId> currentOrbs = null, bool queue = false, bool bypass = false, bool invokeIfOnLastPosition = false)
    {
        return this.invokeHelper.Invoke(currentOrbs, queue, bypass, invokeIfOnLastPosition);
    }

    public override bool UseAbility(Unit9 target, bool queue = false, bool bypass = false)
    {
        return this.Invoke(null, false, bypass) && base.UseAbility(target, queue, bypass);
    }

    internal override void SetOwner(Unit9 owner)
    {
        base.SetOwner(owner);
        this.invokeHelper.SetOwner(owner);
    }
}
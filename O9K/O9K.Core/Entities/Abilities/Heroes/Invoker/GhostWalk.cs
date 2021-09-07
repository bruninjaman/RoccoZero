﻿namespace O9K.Core.Entities.Abilities.Heroes.Invoker;

using System.Collections.Generic;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Entities.Units;

using Helpers;

using Metadata;

[AbilityId(AbilityId.invoker_ghost_walk)]
public class GhostWalk : ActiveAbility, IInvokableAbility
{
    private readonly InvokeHelper<GhostWalk> invokeHelper;

    public GhostWalk(Ability baseAbility)
        : base(baseAbility)
    {
        this.invokeHelper = new InvokeHelper<GhostWalk>(this);
    }

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
    
    public AbilitySlot GetAbilitySlot => invokeHelper.GetAbilitySlot;

    public override bool IsInvisibility { get; } = true;

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

    public AbilityId[] RequiredOrbs { get; } = { AbilityId.invoker_quas, AbilityId.invoker_quas, AbilityId.invoker_wex };

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
        return this.Invoke(null, false, bypass) && base.UseAbility(queue, bypass);
    }

    public override bool UseAbility(bool queue = false, bool bypass = false)
    {
        return this.Invoke(null, false, bypass) && base.UseAbility(queue, bypass);
    }

    internal override void SetOwner(Unit9 owner)
    {
        base.SetOwner(owner);
        this.invokeHelper.SetOwner(owner);
    }
}
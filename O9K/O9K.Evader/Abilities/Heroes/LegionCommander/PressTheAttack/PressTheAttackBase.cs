﻿using O9K.Evader.Abilities.Base.Usable.CounterAbility;

namespace O9K.Evader.Abilities.Heroes.LegionCommander.PressTheAttack;

using Base;
using Base.Evadable;
using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.legion_commander_press_the_attack)]
internal class PressTheAttackBase : EvaderBaseAbility, IEvadable, IUsable<CounterAbility>
{
    public PressTheAttackBase(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new PressTheAttackEvadable(this.Ability, this.Pathfinder, this.Menu);
    }

    public CounterAbility GetUsableAbility()
    {
        return new CounterAbilityWithNotStableMagicImmune(this.Ability, this.Menu);
    }
}
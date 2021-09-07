﻿namespace O9K.Core.Entities.Abilities.Heroes.KeeperOfTheLight;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.keeper_of_the_light_will_o_wisp)]
public class WilloWisp : CircleAbility, IDisable
{
    public WilloWisp(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}
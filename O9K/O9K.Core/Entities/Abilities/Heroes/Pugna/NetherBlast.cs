﻿namespace O9K.Core.Entities.Abilities.Heroes.Pugna;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.pugna_nether_blast)]
public class NetherBlast : CircleAbility, INuke
{
    public NetherBlast(Ability baseAbility)
        : base(baseAbility)
    {
        this.ActivationDelayData = new SpecialData(baseAbility, "delay");
        this.DamageData = new SpecialData(baseAbility, "blast_damage");
        this.RadiusData = new SpecialData(baseAbility, "radius");
    }
}
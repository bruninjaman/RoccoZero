﻿namespace O9K.Core.Entities.Abilities.Heroes.EmberSpirit;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.ember_spirit_fire_remnant)]
public class FireRemnant : CircleAbility
{
    private readonly SpecialData aghanimsSpeedMultiplier;

    public FireRemnant(Ability baseAbility)
        : base(baseAbility)
    {
        this.SpeedData = new SpecialData(baseAbility, "speed_multiplier");
        this.DamageData = new SpecialData(baseAbility, "damage");
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.aghanimsSpeedMultiplier = new SpecialData(baseAbility, "scepter_speed_multiplier");
    }

    public override bool HasAreaOfEffect { get; } = false;

    public override float Speed
    {
        get
        {
            var multiplier = (this.SpeedData.GetValue(this.Level) / 100);

            if (this.Owner.HasAghanimsScepter)
            {
                multiplier *= this.aghanimsSpeedMultiplier.GetValue(this.Level);
            }

            return this.Owner.Speed * multiplier;
        }
    }
}
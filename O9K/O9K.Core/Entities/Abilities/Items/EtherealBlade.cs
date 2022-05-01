﻿using System;

namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Components;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes.Components;

using Entities.Units;

using Helpers;
using Helpers.Damage;

using Metadata;

[AbilityId(AbilityId.item_ethereal_blade)]
public class EtherealBlade : RangedAbility, IShield, IDisable, INuke, IDebuff, IHasDamageAmplify
{
    private readonly SpecialData etherealAmplifierData;

    private readonly SpecialData amplifierData;

    private readonly SpecialData damageMultiplierData;

    public EtherealBlade(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "blast_damage_base");
        this.SpeedData = new SpecialData(baseAbility, "projectile_speed");
        this.etherealAmplifierData = new SpecialData(baseAbility, "ethereal_damage_bonus");
        this.damageMultiplierData = new SpecialData(baseAbility, "blast_agility_multiplier");
        this.amplifierData = new SpecialData(baseAbility, "spell_amp");
    }

    public DamageType AmplifierDamageType { get; } = DamageType.Magical;

    public string[] AmplifierModifierNames { get; } = { "modifier_item_ethereal_blade_ethereal" };

    public AmplifiesDamage AmplifiesDamage { get; } = AmplifiesDamage.Incoming;

    public UnitState AppliesUnitState { get; } = UnitState.Disarmed;

    public override DamageType DamageType { get; } = DamageType.Magical;

    public string DebuffModifierName { get; } = "modifier_item_ethereal_blade_ethereal";

    public bool IsAmplifierAddedToStats { get; } = true;

    public bool IsAmplifierPermanent { get; } = false;

    public string ShieldModifierName { get; } = "modifier_item_ethereal_blade_ethereal";

    public bool ShieldsAlly { get; } = true;

    public bool ShieldsOwner { get; } = true;

    public float AmplifierValue(Unit9 source, Unit9 target)
    {
        var value = 0f;

        if (this.IsUsable)
        {
            value += this.amplifierData.GetValue(this.Level) / 100f;
        }

        value += this.etherealAmplifierData.GetValue(this.Level) / -100;

        return value;
    }

    public override int GetDamage(Unit9 unit)
    {
        var amplify = unit.GetDamageAmplification(this.Owner, this.DamageType, true);

        var block = unit.GetDamageBlock(this.DamageType);
        var damage = this.GetRawDamage(unit);

        var bonusAmplifier = 1f;
        if (!unit.HasModifier(this.AmplifierModifierNames))
        {
            bonusAmplifier += this.AmplifierValue(this.Owner, unit);
        }

        return (int)((damage[this.DamageType] - block) * amplify * bonusAmplifier);
    }

    public override Damage GetRawDamage(Unit9 unit, float? remainingHealth = null)
    {
        var damage = base.GetRawDamage(unit, remainingHealth);
        var multiplier = this.damageMultiplierData.GetValue(this.Level);

        switch (unit.PrimaryAttribute)
        {
            case Attribute.Strength:
                damage[this.DamageType] += multiplier * unit.TotalStrength;
                break;
            case Attribute.Agility:
                damage[this.DamageType] += multiplier * unit.TotalAgility;
                break;
            case Attribute.Intelligence:
                damage[this.DamageType] += multiplier * unit.TotalIntelligence;
                break;
        }

        return damage;
    }
}
﻿namespace O9K.Core.Entities.Abilities.Heroes.Zeus;

using System.Linq;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Entities.Units;

using Helpers;
using Helpers.Damage;

using Managers.Entity;

using Metadata;

[AbilityId(AbilityId.zuus_lightning_bolt)]
public class LightningBolt : RangedAbility, INuke, IDisable
{
    private StaticField staticField;

    public LightningBolt(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "spread_aoe");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;

    public override AbilityBehavior AbilityBehavior
    {
        get
        {
            var behavior = base.AbilityBehavior;
            var talent = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_zeus_5);

            if (talent?.Level > 0)
            {
                behavior = (behavior & ~AbilityBehavior.UnitTarget) | AbilityBehavior.Point;
            }

            return behavior;
        }
    }

    public override Damage GetRawDamage(Unit9 unit, float? remainingHealth = null)
    {
        var damage = base.GetRawDamage(unit, remainingHealth);

        if (this.staticField?.CanBeCasted() == true)
        {
            damage += this.staticField.GetRawDamage(unit, remainingHealth);
        }

        return damage;
    }

    internal Damage GetBaseDamage()
    {
        return new Damage
        {
            [this.DamageType] = this.DamageData.GetValue(this.Level)
        };
    }

    internal override void SetOwner(Unit9 owner)
    {
        base.SetOwner(owner);

        var ability = EntityManager9.BaseAbilities.FirstOrDefault(
            x => x.Id == AbilityId.zuus_static_field && x.Owner?.Handle == owner.Handle);

        if (ability == null)
        {
            return;
        }

        this.staticField = (StaticField)EntityManager9.AddAbility(ability);
    }
}
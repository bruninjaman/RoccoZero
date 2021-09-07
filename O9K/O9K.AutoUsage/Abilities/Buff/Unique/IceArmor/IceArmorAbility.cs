﻿namespace O9K.AutoUsage.Abilities.Buff.Unique.IceArmor;

using System.Collections.Generic;
using System.Linq;

using Core.Entities.Abilities.Base.Types;
using Core.Entities.Metadata;
using Core.Entities.Units;
using Core.Managers.Entity;
using Divine.Extensions;
using Divine.Entity.Entities.Abilities.Components;

using Settings;

[AbilityId(AbilityId.lich_frost_armor)]
internal class IceArmorAbility : BuffAbility
{
    private readonly IceArmorSettings settings;

    public IceArmorAbility(IBuff buff, GroupSettings settings)
        : base(buff)
    {
        this.settings = new IceArmorSettings(settings.Menu, buff);
    }

    public override bool UseAbility(List<Unit9> heroes)
    {
        if (this.Owner.ManaPercentage < this.settings.MpThreshold)
        {
            return false;
        }

        var allies = heroes.Where(x => !x.IsInvulnerable && x.IsAlly(this.Owner)).ToList();
        var enemies = heroes.Where(x => !x.IsInvulnerable && x.IsEnemy(this.Owner)).ToList();

        if (this.settings.UseOnTowers)
        {
            allies.AddRange(EntityManager9.Units.Where(x => x.IsTower && x.IsAlive && x.IsAlly(this.Owner)));
        }

        if (this.settings.UseOnRax)
        {
            allies.AddRange(EntityManager9.Units.Where(x => x.IsBarrack && x.IsAlive && x.IsAlly(this.Owner)));
        }

        foreach (var ally in allies)
        {
            if (!this.settings.IsHeroEnabled(ally.Name) && !ally.IsTower && !ally.IsBarrack && !this.settings.SelfOnly)
            {
                continue;
            }

            var selfTarget = ally.Equals(this.Owner);

            if (selfTarget && !this.Buff.BuffsOwner)
            {
                continue;
            }

            if (!selfTarget && (!this.Buff.BuffsAlly || this.settings.SelfOnly))
            {
                continue;
            }

            if (!this.Ability.CanHit(ally))
            {
                continue;
            }

            var modifier = ally.BaseUnit.GetModifierByName(this.Buff.BuffModifierName);
            if (modifier != null)
            {
                if (this.settings.RenewTime <= 0 || modifier.RemainingTime * 1000 > this.settings.RenewTime)
                {
                    continue;
                }
            }

            if (enemies.Count(x => x.Distance(ally) < this.settings.Distance) < this.settings.EnemiesCount)
            {
                continue;
            }

            if (!this.settings.OnSight && (!this.settings.OnAttack || !ally.IsAttackingHero()))
            {
                return false;
            }

            if (ally.HealthPercentage > this.settings.HpThreshold)
            {
                continue;
            }

            return this.Ability.UseAbility(ally);
        }

        return false;
    }
}
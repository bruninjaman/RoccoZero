﻿namespace O9K.Farm.Units.Base;

using System;
using System.Collections.Generic;
using System.Linq;

using Damage;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Game;
using Divine.Numerics;

using Menu;

using O9K.Core.Entities.Units;
using O9K.Core.Helpers;
using O9K.Core.Helpers.Damage;
using O9K.Core.Managers.Menu.Items;

internal class FarmUnit : IEquatable<FarmUnit>
{
    public FarmUnit(Unit9 unit)
    {
        this.Unit = unit;
        this.IsAlly = unit.IsAlly();
    }

    public Sleeper AttackSleeper { get; } = new();
    
    public Sleeper FakeAttackSleeper { get; } = new();

    public float AttackStartTime { get; set; }

    public virtual bool CanBeKilled
    {
        get
        {
            if (this.IsAlly)
            {
                return this.Unit.HealthPercentage < 50;
            }

            return true;
        }
    }

    public List<UnitDamage> IncomingDamage { get; } = new();

    public bool IsAlly { get; }

    public bool IsControllable { get; protected set; }

    public bool IsDenyEnabled => this.Menu.LastHitMenu.Deny;

    public bool IsHarassEnabled => this.Menu.LastHitMenu.Harass;

    public bool IsHero { get; protected set; }

    public bool IsLastHitEnabled => this.Menu.LastHitMenu.LastHit;

    public bool IsMyHero { get; protected set; }

    public bool IsTower { get; protected set; }

    public bool IsValid => this.Unit.IsValid && this.Unit.IsVisible && this.Unit.IsAlive && !this.Unit.IsInvulnerable;

    public Vector3 LastMovePosition { get; set; } = Vector3.Zero;

    public Sleeper MoveSleeper { get; } = new();

    public FarmUnit Target { get; set; }

    public Unit9 Unit { get; }

    protected UnitMenu Menu { get; private set; }

    public bool Equals(FarmUnit other)
    {
        return this.Unit.Handle == other?.Unit.Handle;
    }

    public virtual UnitDamage AddDamage(FarmUnit target, float attackStartTime, bool addNext, bool forceRanged)
    {
        UnitDamage damage;

        if (this.Unit.IsRanged || forceRanged)
        {
            damage = new RangedDamage(this, target, attackStartTime, 0, 0.05f);  // BEFORE WAS 0.05f
        }
        else
        {
            damage = new MeleeDamage(this, target, attackStartTime, 0.033f); // BEFORE WAS 0.033f
        }

        target.IncomingDamage.Add(damage);

        return damage;
    }

    public virtual bool Attack(FarmUnit target)
    {
        if (this.AttackSleeper.IsSleeping)
        {
            return false;
        }

        if (!this.Unit.Attack(target.Unit))
        {
            return false;
        }

        var ping = GameManager.Ping / 2000;
        var turnTime = this.Unit.GetTurnTime(target.Unit.Position);
        var distance = Math.Max(this.Unit.Distance(target.Unit) - this.Unit.GetAttackRange(target.Unit), 0) / this.Unit.Speed;
        var delay = turnTime + distance + ping + 0.25f;

        var attackPoint = this.Unit.GetAttackPoint(target.Unit);

        if (this.Unit.Abilities.Any(x => x.Id == AbilityId.item_echo_sabre && x.CanBeCasted()))
        {
            attackPoint *= 2.5f;
        }

        this.AttackSleeper.Sleep(this.Unit.GetAttackPoint(target.Unit) + this.Unit.GetAttackBackswing(target.Unit) + delay);
        this.MoveSleeper.Sleep(attackPoint + delay + this.Menu.AdditionalDelay / 1000f);
        this.LastMovePosition = Vector3.Zero;
        this.Target = target;
        // this.AddDamage(target, GameManager.RawGameTime + distance - ping, false, false);

        return true;
    }
    
    public virtual bool FakeAttack(FarmUnit target)
    { 
        if (this.AttackSleeper.IsSleeping)
        {
            return false;
        }
        
        if (this.FakeAttackSleeper.IsSleeping)
        {
            return false;
        }

        if (!this.Unit.Attack(target.Unit))
        {
            return false;
        }

        this.Unit.BaseUnit.Stop();
        var ping = GameManager.Ping / 2000;
        var turnTime = this.Unit.GetTurnTime(target.Unit.Position);
        var distance = Math.Max(this.Unit.Distance(target.Unit) - this.Unit.GetAttackRange(target.Unit), 0) / this.Unit.Speed;
        var delay = turnTime + distance + ping + 0.25f;

        var attackPoint = this.Unit.GetAttackPoint(target.Unit);
        this.AttackSleeper.Sleep(0.2f);
        this.MoveSleeper.Sleep(attackPoint + delay + this.Menu.AdditionalDelay / 1000f);
        this.LastMovePosition = Vector3.Zero;
        this.Target = target;
        
        return true;
    }

    public virtual void AttackStart(IReadOnlyList<FarmUnit> units, float attackStartTime)
    {
        var target = units
            .Where(
                x => x.Unit != this.Unit && x.Unit.Distance(this.Unit) <= this.Unit.GetAttackRange(x.Unit)
                                         && (!x.Unit.IsAlly(this.Unit) || x.Unit.HealthPercentage < 50))
            .OrderBy(x => this.Unit.GetAngle(x.Unit.Position, true))
            .FirstOrDefault(x => this.Unit.GetAngle(x.Unit.Position, true) < 0.2f);

        if (target == null)
        {
            return;
        }

        this.Target = target;
        this.AddDamage(target, attackStartTime, false, false);
    }

    public virtual bool CanAttack(FarmUnit target, float? forceBonusRange = null)
    {
        if (!this.Unit.CanAttack(target.Unit, forceBonusRange ?? this.Menu.BonusRange))
        {
            return false;
        }

        var delay = this.Unit.GetTurnTime(target.Unit.Position);

        if (delay <= 0)
        {
            return !this.AttackSleeper.IsSleeping;
        }

        return this.AttackSleeper.RemainingSleepTime <= delay;
    }

    public virtual bool CanMove()
    {
        if (!this.Unit.CanMove())
        {
            return false;
        }

        return !this.MoveSleeper.IsSleeping;
    }

    public bool CanMoveToMouse()
    {
        if (!this.CanMove())
        {
            return false;
        }

        return true;
    }

    public virtual void CreateMenu(Menu unitsMenu)
    {
        this.Menu = new UnitMenu(this.Unit, unitsMenu);
        this.IsControllable = true;
    }

    public bool Farm(FarmUnit enemy)
    {
        return this.Attack(enemy);
    }
    
    public bool FakeFarm(FarmUnit enemy)
    {
        return this.FakeAttack(enemy);
    }

    public float GetAttackDelay(FarmUnit target)
    {
        var attackPoint = this.Unit.GetAttackPoint(target.Unit);
        var attackRange = this.Unit.GetAttackRange(target.Unit);
        var distance = this.Unit.Distance(target.Unit);
        var distance3D = this.Unit.Distance3D(target.Unit);
        var moveDistance = Math.Max(distance - attackRange, 0);
        var moveTime = moveDistance / this.Unit.Speed;
        var projectileTime = this.Unit.IsRanged ? Math.Max(distance3D - moveDistance, 0) / this.Unit.ProjectileSpeed : 0;
        var turnTime = this.Unit.GetTurnTime(target.Unit.Position);
        var customDelay = this.Menu.AdditionalDelay / 1000f;
        var ping = GameManager.Ping / 2000;

        if (target.IsTower)
        {
            customDelay -= 0.1f;
        }
        else if (target.IsAlly && this.Menu.LastHitMenu.AggressiveDeny)
        {
            customDelay += 0.1f;
        }

        if (this.Unit.IsRanged)
        {
            customDelay -= 0.08f;
        }

        return attackPoint + moveTime + projectileTime + turnTime + ping + customDelay;
    }


    public float GetSimpleAttackDelay(FarmUnit target)
    {
        var attackPoint = this.Unit.GetAttackPoint(target.Unit);
        var distance3D = this.Unit.Distance3D(target.Unit);
        var projectileTime = this.Unit.IsRanged ? Math.Max(distance3D, 0) / this.Unit.ProjectileSpeed : 0;

        return attackPoint + projectileTime;
    }

    public virtual int GetAverageDamage(FarmUnit target)
    {
        return this.Unit.GetAttackDamage(target.Unit, DamageValue.Average);
    }

    public virtual int GetDamage(FarmUnit target)
    {
        return this.Unit.GetAttackDamage(target.Unit);
    }

    public virtual int GetMaxDamage(FarmUnit target)
    {
        return this.Unit.GetAttackDamage(target.Unit, DamageValue.Maximum);
    }

    public float? GetPredictedDeathTime(IReadOnlyList<FarmUnit> units)
    {
        var enemies = units.Where(x => x.Target?.Unit.Equals(this.Unit) == true).ToList();

        if (enemies.Count == 0)
        {
            return null;
        }

        var time = GameManager.RawGameTime;
        var health = this.Unit.Health;
        var deathTime = 0;

        while (health > 0)
        {
            foreach (var enemy in enemies)
            {
                var dps = enemy.Unit.AttacksPerSecond * enemy.Unit.GetAttackDamage(this.Unit, DamageValue.Maximum);

                if (dps < 0)
                {
                    return null;
                }

                health -= dps;
            }

            deathTime++;
        }

        return time + deathTime;
    }

    public float GetPredictedHealth(FarmUnit source, float delay)
    {
        var time = GameManager.RawGameTime;
        var hitTime = time + delay;
        var health = this.Unit.Health;
        var regen = (int)Math.Ceiling(this.Unit.HealthRegeneration * delay);

        foreach (var damage in this.IncomingDamage)
        {
            if (damage.MinDamage <= 0 || damage.Source.Equals(source))
            {
                continue;
            }

            if (damage.WillHit(time, hitTime))
            {
                health -= damage.MinDamage;
            }
        }

        return health + regen;
    }

    public float GetPredictedHealth(float delay)
    {
        var time = GameManager.RawGameTime;
        var hitTime = time + delay;
        var health = this.Unit.Health;
        var regen = (int)Math.Ceiling(this.Unit.HealthRegeneration * delay);

        foreach (var damage in this.IncomingDamage)
        {
            if (damage.MinDamage <= 0)
            {
                continue;
            }

            if (damage.WillHit(time, hitTime))
            {
                health -= damage.MinDamage;
            }
        }

        return health + regen;
    }

    public bool Harass(FarmUnit target)
    {
        if (this.AttackSleeper.IsSleeping)
        {
            return false;
        }
        
        if (!this.Unit.Attack(target.Unit))
        {
            return false;
        }

        var ping = GameManager.Ping / 2000 + 0.06f;
        var turnTime = this.Unit.GetTurnTime(target.Unit.Position);
        var distance = Math.Max(this.Unit.Distance(target.Unit) - this.Unit.GetAttackRange(target.Unit), 0) / this.Unit.Speed;
        var delay = turnTime + distance + ping + 0.25f;

        var attackPoint = this.Unit.GetAttackPoint(target.Unit);

        if (this.Unit.Abilities.Any(x => x.Id == AbilityId.item_echo_sabre && x.CanBeCasted()))
        {
            attackPoint *= 2.5f;
        }

        this.AttackSleeper.Sleep(this.Unit.GetAttackPoint(target.Unit) + this.Unit.GetAttackBackswing(target.Unit) + delay);
        this.MoveSleeper.Sleep(attackPoint + delay + this.Menu.AdditionalDelay / 1000f);
        this.LastMovePosition = Vector3.Zero;

        return true;
    }

    public void Stop()
    {
        this.Unit.Stop();
        this.ResetSleepers();
    }

    public void ResetSleepers()
    {
        this.AttackSleeper.Reset();
        this.MoveSleeper.Reset();
    }
}
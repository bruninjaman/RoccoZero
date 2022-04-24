using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using InvokerAnnihilation.Constants;
using Sleeper = Divine.Helpers.Sleeper;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseAbstractAbility : IAbility, IExecutableAbilityInCombo
{
    protected BaseAbstractAbility(Ability baseAbility)
    {
        BaseAbility = baseAbility;
        Owner = (Hero?) baseAbility.Owner!;
        AbilityId = baseAbility.Id;
        AbilitySleeper = new Sleeper();
    }

    protected BaseAbstractAbility(Hero owner, AbilityId abilityId)
    {
        Owner = owner;
        AbilityId = abilityId;
        AbilitySleeper = new Sleeper();
    }

    public Sleeper AbilitySleeper { get; set; }
    
    public Ability? BaseAbility { get; set; }
    public AbilityId AbilityId { get; set; }
    public Hero Owner { get; }
    public bool IsValid => BaseAbility != null && BaseAbility.IsValid && BaseAbility.IsActivated;

    public virtual bool Cast()
    {
        throw new NotImplementedException();
    }

    public virtual bool CanBeCasted()
    {
        if (!IsValid)
            return false;
        if (Owner.IsStunned() || !Owner.IsAlive || Owner.IsMuted())
        {
            return false;
        }

        if (OnCooldown || HasNoMana || NotLearned)
        {
            return false;
        }

        return true;
    }

    public virtual bool Cast(Vector3 targetPosition, Unit target)
    {
        throw new NotImplementedException();
    }

    public virtual bool CanBeCasted(Vector3 targetPosition)
    {
        throw new NotImplementedException();
    }

    public virtual bool Cast(Unit target)
    {
        throw new NotImplementedException();
    }

    public virtual bool CanBeCasted(Unit? target)
    {
        throw new NotImplementedException();
    }

    public bool OnCooldown => !IsValid || RemainingCooldown > 0.01f;
    public bool HasNoMana => !IsValid || BaseAbility!.ManaCost > Owner.Mana;
    public bool NotLearned => !IsValid || BaseAbility!.Level == 0;
    public float ManaCost => IsValid ? BaseAbility!.ManaCost : 0;
    public float RemainingCooldown => BaseAbility!.Cooldown;
    public uint Level => IsValid ? BaseAbility!.Level : 0;

    public void SetAbility(Ability? ability)
    {
        BaseAbility = ability;
        if (ability != null)
            AbilityId = ability.Id;
    }

    public virtual bool ShouldCast(Unit? target)
    {
        if (target == null)
        {
            return false;
        }

        return true;
    }

    public virtual bool ChainCast(Unit? target, Vector3 forcedTargetPosition = default, bool checkForStun = true,
        bool checkForInvul = true)
    {
        return false;
    }

    public virtual float GetDelay { get; } = GameManager.Ping / 2000;
    public virtual bool UseOnMagicImmuneTarget { get; } = false;

    public virtual float GetCastDelay(Unit unit)
    {
        if (Owner.Equals(unit))
        {
            return GetCastDelay();
        }

        return GetCastDelay(unit.Position);
    }

    public virtual float GetCastDelay(Vector3 position)
    {
        return GetCastDelay() + Owner.GetTurnTime(position);
    }

    public virtual float CastPoint => IsValid ? BaseAbility!.AbilityData.GetCastPoint(0) : 0;

    public virtual float GetCastDelay()
    {
        return CastPoint + InputLag;
    }

    public virtual float GetHitTime(Unit unit)
    {
        if (Owner.Handle.Equals(unit.Handle))
        {
            return GetCastDelay() + ActivationDelay;
        }

        return GetHitTime(unit.Position);
    }

    public virtual float ActivationDelay => this.ActivationDelayData?.GetValue(this.Level) ?? 0;

    public virtual float GetHitTime(Vector3 position)
    {
        var time = GetCastDelay(position) + ActivationDelay;

        if (Speed > 0)
        {
            return time + (Owner.Distance2D(position) / Speed);
        }

        return time;
    }

    protected SpecialData SpeedData { get; set; }

    protected SpecialData EndRadiusData { get; set; }

    protected SpecialData RadiusData { get; set; }
    protected SpecialData ActivationDelayData;
    protected SpecialData DamageData;
    protected SpecialData DurationData;
    protected SpecialData RangeData;

    public virtual float Speed => SpeedData?.GetValue(Level) ?? 0;

    public float InputLag => GameManager.Ping / 2000;
    public virtual float Radius => this.RadiusData?.GetValue(this.Level) ?? 0;
}
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;
using InvokerAnnihilation.Constants;

namespace InvokerAnnihilation.Abilities.MainAbilities;

[Ability(AbilityId.invoker_sun_strike,new []{ AbilityId.invoker_exort, AbilityId.invoker_exort, AbilityId.invoker_exort})]
public class SunStrike : BaseInvokablePointAbstractAbility
{
    public SunStrike(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
        this.ActivationDelayData = new SpecialData(BaseAbility, "delay");
        this.RadiusData = new SpecialData(BaseAbility, "area_of_effect");
        this.DamageData = new SpecialData(BaseAbility, "damage");
    }

    public override float CastRange { get; } = int.MaxValue;
    public override float GetDelay => GameManager.Ping/2000 +  1.7f + 0.05f;

    public override bool Cast(Vector3 targetPosition, Unit target)
    {
        if (IsValid && CanBeCasted(targetPosition) && CanBeCasted(target))
        {
            if (!IsInvoked)
            {
                if (CanBeInvoked())
                {
                    var invoked = Invoke();
                    if (!invoked)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return ChainCast(target, targetPosition);
        }

        return false;
    }
    
    public override bool ChainCast(Unit? target, Vector3 forcedTargetPosition = default, bool checkForStun = true, bool checkForInvul = true)
    {
        if (target == null)
            return false;
        var targetPosition = forcedTargetPosition.IsZero ? target.Position : forcedTargetPosition;
        
        var isInvul = target.TargetIsInvul(out var remainingInvul);
        var isStunned = target.IsStunned(out var remainingStun);
        var hitTime = GetHitTime(targetPosition);
        var canCastForStun = false;
        var canCastForInvul = false;
        if (isStunned)
        {
            canCastForStun = remainingStun >= hitTime;
            if (checkForStun && canCastForStun)
            {
                return BaseAbility!.Cast(targetPosition);
            }
            else
            {
                
            }
        }
        
        if (isInvul)
        {
            canCastForInvul = remainingInvul <= hitTime;
            if (checkForInvul && canCastForInvul)
            {
                if (forcedTargetPosition == Vector3.Zero)
                {
                    // Console.WriteLine("cast to owner");
                    return BaseAbility!.Cast(Owner);
                }
                else
                {
                    return BaseAbility!.Cast(targetPosition);
                }
            }
            else
            {
                
            }
        }

        return true;
    }
    
    public override bool UseOnMagicImmuneTarget => true;
}
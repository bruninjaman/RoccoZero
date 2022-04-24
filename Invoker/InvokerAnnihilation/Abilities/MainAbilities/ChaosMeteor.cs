using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Constants;
using InvokerAnnihilation.Attributes;

namespace InvokerAnnihilation.Abilities.MainAbilities;

[Ability(AbilityId.invoker_chaos_meteor, new []{AbilityId.invoker_exort, AbilityId.invoker_exort, AbilityId.invoker_wex})]
public class ChaosMeteor : BaseInvokablePointAbstractAbility
{
    public ChaosMeteor(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
        this.ActivationDelayData = new SpecialData(BaseAbility, "land_time");
        this.RadiusData = new SpecialData(BaseAbility, "area_of_effect");
        // this.RangeData = new SpecialData(BaseAbility, "travel_distance");
        // this.SpeedData = new SpecialData(BaseAbility, "travel_speed");
    }

    public override float GetDelay => base.GetDelay + 0.05f + 1.3f;

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

            var dif = targetPosition.Extend(Owner.Position, Radius);

            return ChainCast(target, dif);
        }

        return false;
    }
    
    public override bool ChainCast(Unit? target, Vector3 forcedTargetPosition = default, bool checkForStun = true,
        bool checkForInvul = true)
    {
        if (target == null)
            return false;
        var targetPosition = forcedTargetPosition.IsZero ? target.Position : forcedTargetPosition;

        var isInvul = target.TargetIsInvul(out var remainingInvul);
        var isStunned = target.IsStunned(out var remainingStun);
        var hitTime = GetHitTime(targetPosition);
        var canCastForStun = false;
        var canCastForInvul = false;
        if (isStunned && !isInvul)
        {
            return BaseAbility!.Cast(targetPosition);
        }

        if (isInvul)
        {
            canCastForInvul = remainingInvul <= hitTime;
            if (checkForInvul && canCastForInvul)
            {
                return BaseAbility!.Cast(targetPosition);
            }
            else
            {
            }
        }
        else if (target.HasAnyModifiers(Consts.MeteorBurn))
        {
            return BaseAbility!.Cast(targetPosition);
        }


        return true;
    }
}
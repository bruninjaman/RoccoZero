using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;
using InvokerAnnihilation.Constants;
using SpecialData = InvokerAnnihilation.Constants.SpecialData;

namespace InvokerAnnihilation.Abilities.MainAbilities;

[Ability(AbilityId.invoker_deafening_blast,
    new[] {AbilityId.invoker_quas, AbilityId.invoker_wex, AbilityId.invoker_exort})]
public class Blast : BaseInvokablePointAbstractAbility
{
    public Blast(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
        RadiusData = new SpecialData(BaseAbility, "radius_start");
        EndRadiusData = new SpecialData(BaseAbility, "radius_end");
        SpeedData = new SpecialData(BaseAbility, "travel_speed");
    }

    public float ProjectileSpeed { get; set; }
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

            return ChainCast(target);
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
            return !BaseAbility!.Cast(targetPosition);
        }

        if (isInvul)
        {
            canCastForInvul = remainingInvul <= hitTime;
            if (checkForInvul && canCastForInvul)
            {
                return !BaseAbility!.Cast(targetPosition);
            }
            else
            {
            }
        }
        else if (target.HasAnyModifiers(Consts.MeteorBurn))
        {
            return !BaseAbility!.Cast(targetPosition);
        }


        return true;
    }
}
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseInvokableTargetAbstractAbility : BaseInvokableAbstractAbility, ITargetAbility
{
    protected BaseInvokableTargetAbstractAbility(Ability baseAbility, AbilityId[] spheres) : base(baseAbility, spheres)
    {
    }

    public override bool Cast(Unit target)
    {
        if (IsValid && CanBeCasted(target))
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
            var castResult = BaseAbility!.Cast(target);
            // Console.WriteLine($"[{AbilityId}] castResult: {castResult}");
            return castResult;
        }

        return false;
    }

    public override bool CanBeCasted(Unit? target)
    {
        if (base.CanBeCasted() && target != null && target.IsValidTarget(CastRange, false) &&
            (IsInvoked || CanBeInvoked()) && (!target.IsMagicImmune() || UseOnMagicImmuneTarget))
        {
            return true;
        }

        return false;
    }

    public virtual bool HitEnemy { get; } = false;
    public virtual bool HitAlly { get; } = false;
    public float CastRange => BaseAbility!.GetCastRange();
}
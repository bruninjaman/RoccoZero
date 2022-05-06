using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseInvokableNoTargetAbstractAbility : BaseInvokableAbstractAbility, INoTargetAbility
{
    protected BaseInvokableNoTargetAbstractAbility(Ability baseAbility, AbilityId[] spheres) : base(baseAbility,
        spheres)
    {
    }

    public override bool Cast()
    {
        if (IsValid && CanBeCasted())
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

            return !BaseAbility!.Cast();
        }

        return false;
    }
}
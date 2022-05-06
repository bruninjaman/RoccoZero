using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;
using InvokerAnnihilation.Constants;

namespace InvokerAnnihilation.Abilities.MainAbilities;

[Ability(AbilityId.invoker_emp, new []{AbilityId.invoker_wex, AbilityId.invoker_wex, AbilityId.invoker_wex})]
public class Emp : BaseInvokablePointAbstractAbility
{
    public Emp(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
        this.ActivationDelayData = new SpecialData(BaseAbility, "delay");
        this.RadiusData = new SpecialData(BaseAbility, "area_of_effect");
        this.DamageData = new SpecialData(BaseAbility, "mana_burned");
    }
    
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

    // public override bool UseOnMagicImmuneTarget => true;
}
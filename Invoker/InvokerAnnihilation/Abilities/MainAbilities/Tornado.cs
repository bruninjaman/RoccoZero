using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;
using InvokerAnnihilation.Constants;

namespace InvokerAnnihilation.Abilities.MainAbilities;

[Ability(AbilityId.invoker_tornado, new[] {AbilityId.invoker_wex, AbilityId.invoker_wex, AbilityId.invoker_quas})]
public class Tornado : BaseInvokablePointAbstractAbility
{
    public Tornado(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
        this.RadiusData = new SpecialData(BaseAbility, "area_of_effect");
        this.DamageData = new SpecialData(BaseAbility, "wex_damage");
        this.RangeData = new SpecialData(BaseAbility, "travel_distance");
        this.SpeedData = new SpecialData(BaseAbility, "travel_speed");
    }

    private List<string> IgnoreModifiers = new List<string>()
    {
        "modifier_legion_commander_duel",
        Consts.MeteorBurn
    };

    public override bool ShouldCast(Unit? target)
    {
        if (target == null)
            return false;
        if (target.IsMagicImmune())
            return false;
        if (CanBeCasted(target.Position))
        {
            return true;
        }

        return false;
    }

    public override bool Cast(Vector3 targetPosition, Unit target)
    {
        // return base.Cast(targetPosition, target);
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

            if (target.HasAnyModifiers(IgnoreModifiers.ToArray()) ||
                (target.IsStunned(out var remaining) && remaining >= 1.5f))
            {
                return true;
            }

            if (target.IsInvulnerable())
            {
                return ChainCast(target);
            }

            return BaseAbility!.Cast(targetPosition);
        }

        return false;
    }
}
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Helpers;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Abilities.MainAbilities;
using InvokerAnnihilation.Feature.ComboConstructor;

namespace InvokerAnnihilation.Feature.ComboExecutor;

public class AbilityExecutor : IAbilityExecutor
{
    private readonly ComboConstructorFeature _comboConstructorFeature;

    public AbilityExecutor(ComboConstructorFeature comboConstructorFeature)
    {
        _comboConstructorFeature = comboConstructorFeature;
    }

    public bool CastAbility(IAbility abilityToCast, Hero target)
    {
        switch (abilityToCast)
        {
            case ITargetAbility {HitEnemy: true} targetAbility:
            {
                return targetAbility.CanBeCasted(target) && targetAbility.Cast(target);
            }
            case ITargetAbility targetAbility when targetAbility.CanBeCasted(abilityToCast.Owner):
            {
                return targetAbility.Cast(abilityToCast.Owner);
            }
            case INoTargetAbility noTargetAbility when abilityToCast.CanBeCasted():
            {
                return noTargetAbility.Cast();
            }
            case IPointAbility pointAbility when pointAbility.CanBeCasted(target.Position):
            {
                if (abilityToCast is SunStrike sunStrike)
                {
                    var isCataclysm = _comboConstructorFeature.CurrentBuilder.CataclysmInCombo.IsActive && abilityToCast.Owner.HasAghanimsScepter();
                    var result = pointAbility.Cast(isCataclysm ? Vector3.Zero : target.Position, target);
                    if (!result)
                    {
                        MultiSleeper<string>.Sleep("afterSunStrike", sunStrike.GetHitTime(target.Position) * 1000 + 150);
                    }
                    return result;
                }
                else if (abilityToCast is ChaosMeteor chaosMeteor)
                {
                    if (MultiSleeper<string>.Sleeping("afterTornado"))
                    {
                        return true;
                    }
                    var result = pointAbility.Cast(target.Position, target);
                    if (!result)
                    {
                        MultiSleeper<string>.Sleep("afterChaos", chaosMeteor.GetHitTime(target.Position) * 1000+ 500);
                    }

                    return result;
                }
                else if (abilityToCast is Tornado tornado)
                {
                    if (MultiSleeper<string>.Sleeping("afterChaos") || MultiSleeper<string>.Sleeping("afterSunStrike"))
                    {
                        return true;
                    }
                    var result = pointAbility.Cast(target.Position, target);
                    if (!result)
                    {
                        MultiSleeper<string>.Sleep("afterTornado", tornado.GetHitTime(target.Position) * 1000 + 500);
                    }

                    return result;
                }
                else
                {
                    return pointAbility.Cast(target.Position, target);
                }
                
            }
        }

        return false;
    }
}
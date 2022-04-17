using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Extensions;
using Divine.Game;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.Processor;

public class InvokeProcessor : IInvokeProcessor
{
    private readonly BaseInvokableAbstractAbility _ability;

    public InvokeProcessor(BaseInvokableAbstractAbility ability)
    {
        _ability = ability;

        InvokerQuas = ability.Owner.GetAbilityById(AbilityId.invoker_quas);
        InvokerWex = ability.Owner.GetAbilityById(AbilityId.invoker_wex);
        InvokerExort = ability.Owner.GetAbilityById(AbilityId.invoker_exort);
        InvokerInvoke = ability.Owner.GetAbilityById(AbilityId.invoker_invoke);

        AbilityDictionary = new Dictionary<AbilityId, Ability>()
        {
            {AbilityId.invoker_quas, InvokerQuas},
            {AbilityId.invoker_wex, InvokerWex},
            {AbilityId.invoker_exort, InvokerExort}
        };
    }

    public Dictionary<AbilityId,Ability> AbilityDictionary { get; set; }

    public Ability InvokerQuas { get; set; }

    public Ability InvokerWex { get; set; }

    public Ability InvokerExort { get; set; }

    public Ability InvokerInvoke { get; set; }

    public bool CanBeInvoked(bool andCasted)
    {
        if (InvokerInvoke.Cooldown > 0)
        {
            return false;
        }
        var neededSpheres = _ability.Spheres.Distinct();
        return neededSpheres.All(neededSphere => AbilityDictionary[neededSphere].Level != 0) && (!andCasted || _ability.ManaCost < _ability.Owner.Mana);
    }
    
    public float InvokedTime { get; set; }
    
    public bool IsInvoked()
    {
        if (!_ability.BaseAbility!.IsHidden)
        {
            return true;
        }
        
        return (InvokedTime + 0.5f) > GameManager.RawGameTime;
    }

    public bool Invoke()
    {
        if (!CanBeInvoked(false))
        {
            return false;
        }
        var casted= _ability.Spheres.Select(abilitySphere => AbilityDictionary[abilitySphere].Cast()).All(casted => casted) && InvokerInvoke.Cast();

        if (casted)
        {
            InvokedTime = GameManager.RawGameTime;
            return true;
        }

        return false;
    }
}
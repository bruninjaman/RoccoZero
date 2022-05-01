using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;

namespace InvokerAnnihilation.Abilities.MainAbilities;


[Ability(AbilityId.invoker_cold_snap, new []{AbilityId.invoker_quas, AbilityId.invoker_quas, AbilityId.invoker_quas})]
public class ColdSnap : BaseInvokableTargetAbstractAbility
{
    public ColdSnap(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }

    public override bool HitEnemy => true;
}
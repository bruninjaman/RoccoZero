using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;

namespace InvokerAnnihilation.Abilities.MainAbilities;

[Ability(AbilityId.invoker_ghost_walk, new []{AbilityId.invoker_quas, AbilityId.invoker_quas, AbilityId.invoker_wex})]
public class GhostWalk : BaseInvokableNoTargetAbstractAbility
{
    public GhostWalk(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }
    
    public override bool UseOnMagicImmuneTarget => true;
}
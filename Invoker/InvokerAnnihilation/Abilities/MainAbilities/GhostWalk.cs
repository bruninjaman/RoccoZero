using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities;

public class GhostWalk : BaseInvokableNoTargetAbstractAbility
{
    public GhostWalk(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }
}
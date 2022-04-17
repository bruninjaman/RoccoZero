using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities;

public class ColdSnap : BaseInvokableTargetAbstractAbility
{
    public ColdSnap(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }

    public override bool HitEnemy => true;
}
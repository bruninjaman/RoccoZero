using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities;

public class ForgeSpirit : BaseInvokableNoTargetAbstractAbility
{
    public ForgeSpirit(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }
}
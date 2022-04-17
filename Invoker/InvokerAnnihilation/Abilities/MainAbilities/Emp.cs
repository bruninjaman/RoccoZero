using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities;

public class Emp : BaseInvokablePointAbstractAbility
{
    public Emp(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }
}
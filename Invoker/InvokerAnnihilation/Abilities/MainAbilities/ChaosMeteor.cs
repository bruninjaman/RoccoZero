using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities;

public class ChaosMeteor : BaseInvokablePointAbstractAbility
{
    public ChaosMeteor(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }
}
using Divine.Entity.Entities.Units.Heroes;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Feature.ComboExecutor;

public interface IAbilityExecutor
{
    bool CastAbility(IAbility ability, Hero target);
    // bool CastAbility(Vector3 position);
    // bool CastAbility();
}
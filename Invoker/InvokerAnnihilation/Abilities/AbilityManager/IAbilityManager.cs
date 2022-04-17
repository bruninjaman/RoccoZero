using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.AbilityManager;

public interface IAbilityManager
{
    IAbility? GetAbility(AbilityId abilityId);
}
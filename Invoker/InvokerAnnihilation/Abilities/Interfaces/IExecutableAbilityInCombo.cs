using Divine.Entity.Entities.Units;

namespace InvokerAnnihilation.Abilities.Interfaces;

public interface IExecutableAbilityInCombo
{
    bool ShouldCast(Unit? target);
}
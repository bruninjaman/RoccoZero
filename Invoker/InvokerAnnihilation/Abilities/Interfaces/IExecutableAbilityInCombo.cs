using Divine.Entity.Entities.Units;
using Divine.Numerics;

namespace InvokerAnnihilation.Abilities.Interfaces;

public interface IExecutableAbilityInCombo
{
    bool ShouldCast(Unit? target);
    bool ChainCast(Unit? target, Vector3 forcedTargetPosition = default, bool checkForStun = true, bool checkForInvul = true);
    float GetDelay { get; }
    bool UseOnMagicImmuneTarget { get; }
}
using Divine.Entity.Entities.Units;

namespace Farmling.Interfaces;

public interface IDamageCalculateService
{
    public float GetDamage(Unit attacker, Unit target);
}

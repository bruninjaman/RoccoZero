using Divine.Entity.Entities.Units;

namespace InvokerAnnihilation.Abilities.Interfaces;

public interface ITargetAbility
{
    bool Cast(Unit target);
    bool CanBeCasted(Unit? target);
    bool HitEnemy { get; }
    bool HitAlly { get; }
    float CastRange { get; }
}
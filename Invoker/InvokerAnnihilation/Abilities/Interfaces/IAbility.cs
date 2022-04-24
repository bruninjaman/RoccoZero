using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Numerics;

namespace InvokerAnnihilation.Abilities.Interfaces;

public interface IAbility
{
    Ability? BaseAbility { get; set; }
    AbilityId AbilityId { get; set; }
    Hero Owner { get; }
    bool IsValid { get; }
    
    bool OnCooldown { get; }
    bool HasNoMana { get; }
    bool NotLearned { get; }
    float ManaCost { get; }
    float RemainingCooldown { get; }
    uint Level { get; }
    void SetAbility(Ability? ability);
    
    bool Cast();
    bool CanBeCasted();
    
    bool Cast(Vector3 targetPosition, Unit target);
    bool CanBeCasted(Vector3 targetPosition);
    
    bool Cast(Unit target);
    bool CanBeCasted(Unit? target);
}
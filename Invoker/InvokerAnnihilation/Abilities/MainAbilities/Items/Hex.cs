using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities.Items;

public class Hex : BaseItemTargetAbility
{
    public Hex(Ability baseAbility) : base(baseAbility)
    {
    }

    public Hex(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }
}
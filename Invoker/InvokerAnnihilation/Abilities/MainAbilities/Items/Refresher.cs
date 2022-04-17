using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities.Items;

public class Refresher : BaseItemNoTargetAbility
{
    public Refresher(Ability baseAbility) : base(baseAbility)
    {
    }

    public Refresher(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }
}
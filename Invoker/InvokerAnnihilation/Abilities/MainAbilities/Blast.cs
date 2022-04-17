using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities;

public class Blast : BaseInvokablePointAbstractAbility
{
    public Blast(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }
}

// public class Blink : BaseAbstractAbility
// {
//     public override bool IsItem { get; set; } = true;
//
//     public Blink(Ability ability) : base(ability)
//     {
//     }
// }
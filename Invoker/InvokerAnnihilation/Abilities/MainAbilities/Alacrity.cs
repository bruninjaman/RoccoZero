using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities;

public class Alacrity : BaseInvokableTargetAbstractAbility
{
    public Alacrity(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }

    public override bool HitAlly => true;
}
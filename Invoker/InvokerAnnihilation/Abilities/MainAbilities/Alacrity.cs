using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;

namespace InvokerAnnihilation.Abilities.MainAbilities;

[Ability(AbilityId.invoker_alacrity, new []{AbilityId.invoker_wex, AbilityId.invoker_wex, AbilityId.invoker_exort})]
public class Alacrity : BaseInvokableTargetAbstractAbility
{
    public Alacrity(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }

    public override bool HitAlly => true;
    
    public override bool UseOnMagicImmuneTarget => true;
}
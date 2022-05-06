using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;

namespace InvokerAnnihilation.Abilities.MainAbilities;

[Ability(AbilityId.invoker_forge_spirit, new []{AbilityId.invoker_exort, AbilityId.invoker_exort, AbilityId.invoker_quas})]
public class ForgeSpirit : BaseInvokableNoTargetAbstractAbility
{
    public ForgeSpirit(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }
    
    public override bool UseOnMagicImmuneTarget => true;
}
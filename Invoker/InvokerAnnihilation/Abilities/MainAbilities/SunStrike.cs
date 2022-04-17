using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities;

public class SunStrike : BaseInvokablePointAbstractAbility
{
    public SunStrike(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }

    public override float CastRange { get; } = int.MaxValue;
}
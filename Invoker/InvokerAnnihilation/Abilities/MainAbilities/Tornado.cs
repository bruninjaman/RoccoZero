using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities;

public class Tornado : BaseInvokablePointAbstractAbility
{
    public Tornado(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }

    public override bool ShouldCast(Unit? target)
    {
        if (target == null)
            return false;
        if (target.IsMagicImmune())
            return false;
        if (this.CanBeCasted(target.Position))
        {
            return true;
        }

        return false;
    }
}
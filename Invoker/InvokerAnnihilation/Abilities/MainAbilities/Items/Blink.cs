using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;

namespace InvokerAnnihilation.Abilities.MainAbilities.Items;

[Ability(AbilityId.item_blink, AbilityId.item_arcane_blink, AbilityId.item_overwhelming_blink, AbilityId.item_swift_blink)]
public class Blink : BaseItemPointAbility
{
    public Blink(Ability baseAbility) : base(baseAbility)
    {
    }

    public Blink(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }

    public override bool Cast(Vector3 targetPosition, Unit target)
    {
        var mePos = Owner.Position;
        var pos = targetPosition.Extend(mePos, 200);
        return BaseAbility.Cast(pos);
    }

    public override bool CanBeCasted(Vector3 targetPosition)
    {
        if (base.CanBeCasted() && !Owner.IsInRange(targetPosition, 200) && Owner.IsInRange(targetPosition, 2500))
        {
            return true;
        }
        
        return false;
        // return base.CanBeCasted(targetPosition);
    }

    public override float CastRange { get; } = 1200;
}
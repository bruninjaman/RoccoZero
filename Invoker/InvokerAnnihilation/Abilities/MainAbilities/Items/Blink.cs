using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities.Items;

public class Blink : BaseItemPointAbility
{
    public Blink(Ability baseAbility) : base(baseAbility)
    {
    }

    public Blink(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }

    public override bool Cast(Vector3 targetPosition)
    {
        Console.WriteLine("Blink");
        var mePos = Owner.Position;
        var pos = targetPosition.Extend(mePos, 200);
        return BaseAbility.Cast(pos);
    }

    public override bool CanBeCasted(Vector3 targetPosition)
    {
        if (base.CanBeCasted() && !Owner.IsInRange(targetPosition, 400) && !Owner.IsInRange(targetPosition, 1500))
        {
            return true;
        }

        return false;
    }

    public override float CastRange { get; } = 1200;
}
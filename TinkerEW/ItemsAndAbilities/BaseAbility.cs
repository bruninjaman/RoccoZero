using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Numerics;

namespace TinkerEW.ItemsAndAbilities;

internal class BaseAbility
{
    private Ability? Ability;

    public BaseAbility()
    {

    }

    public BaseAbility(Ability? ability)
    {
        Ability = ability;
    }

    public void Update(Ability? ability)
    {
        Ability = ability;
    }

    public Ability? GetAbility()
    {
        return Ability;
    }

    public virtual bool CanBeCasted()
    {
        if (Ability == null)
            return false;

        if (Ability.Cooldown > 0)
            return false;

        if (Ability.Owner is not Hero owner)
            return false;

        if (((Hero)Ability.Owner).Mana < Ability.ManaCost)
            return false;

        if (!owner.IsAlive)
            return false;

        if (Ability.IsChanneling && Ability.CastPoint != 0f && ((Ability.AbilityBehavior & AbilityBehavior.Immediate) == 0))
        {
            return false;
        }

        if (owner.IsChanneling() && Ability.CastPoint != 0f && ((Ability.AbilityBehavior & AbilityBehavior.Immediate) == 0))
        {
            return false;
        }

        if (((Ability.AbilityBehavior & AbilityBehavior.IgnoreChannel) == 0) && owner.IsChanneling())
        {
            return false;
        }

        if (owner.IsStunned())
        {
            return false;
        }

        if (((Ability.AbilityBehavior & AbilityBehavior.RootDisables) == 0) && (owner.IsRooted() || owner.HasModifier("modifier_slark_pounce_leash")))
        {
            return false;
        }

        if ((Ability.Type == EntityType.Ability && owner.IsMuted()) || (Ability.Type == EntityType.Ability && owner.IsSilenced()))
        {
            return false;
        }

        return true;
    }

    public virtual bool Cast(Vector3 position, bool queue = false, bool bypass = false)
    {

        var result = false;

        if (Ability == null)
            return result;

        result = Ability.Cast(position, queue, bypass);

        return result;
    }

    public virtual bool Cast(Unit unit, bool queue = false, bool bypass = false)
    {

        var result = false;

        if (Ability == null || !unit.IsVisible || !unit.IsAlive)
            return result;

        result = Ability.Cast(unit, queue, bypass);

        return result;
    }

    public virtual bool Cast(bool queue = false, bool bypass = false)
    {

        var result = false;

        if (Ability == null)
            return result;

        result = Ability.Cast(queue, bypass);

        return result;
    }
}

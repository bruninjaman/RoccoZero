namespace O9K.AIO.Abilities;

using System.Collections.Generic;

using Core.Entities.Abilities.Base;
using Core.Entities.Abilities.Base.Types;
using Core.Entities.Units;
using Core.Helpers;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Menus;

using Modes.Combo;

using TargetManager;

internal abstract class UsableAbility
{
    protected UsableAbility(ActiveAbility ability)
    {
        this.Ability = ability;
        this.Owner = ability.Owner;
    }

    public ActiveAbility Ability { get; }

    public Sleeper OrbwalkSleeper { get; set; }

    public Sleeper Sleeper { get; set; }

    protected Unit9 Owner { get; }

    public virtual bool CanBeCasted(TargetManager targetManager, bool channelingCheck, IComboModeMenu comboMenu)
    {
        if (this.Sleeper.IsSleeping)
        {
            return false;
        }

        return this.Ability.CanBeCasted(channelingCheck);
    }

    public virtual bool CanHit(TargetManager targetManager, IComboModeMenu comboMenu)
    {
        return this.Ability.CanHit(targetManager.Target);
    }

    public abstract bool ForceUseAbility(TargetManager targetManager, Sleeper comboSleeper);

    public virtual UsableAbilityMenu GetAbilityMenu(string simplifiedName)
    {
        return null;
    }

    public abstract bool ShouldCast(TargetManager targetManager);

    public virtual bool ShouldConditionCast(TargetManager targetManager, IComboModeMenu menu, List<UsableAbility> usableAbilities)
    {
        return true;
    }

    public abstract bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe);

    protected virtual bool ChainStun(Unit9 target, bool invulnerability)
    {
        float? immobile = null;

        if (this.Ability is IDisable disable)
        {
            var appliesUnitState = disable.AppliesUnitState;

            if ((appliesUnitState & UnitState.Hexed) == UnitState.Hexed || (appliesUnitState & UnitState.Stunned) == UnitState.Stunned)
            {
                immobile = target.GetStrongDisableDuration();
            }
        }

        if (!immobile.HasValue)
        {
            immobile = invulnerability ? target.GetInvulnerabilityDuration() : target.GetImmobilityDuration();
        }

        if (immobile <= 0)
        {
            return false;
        }

        float hitTime = this.Ability.GetHitTime(target);

        if (this.Ability.Id == AbilityId.item_sheepstick)
        {
            hitTime += 0.1f;
        }
        
        if (target.IsInvulnerable)
        {
            hitTime -= 0.1f;
        }

        return hitTime > immobile;
    }
}
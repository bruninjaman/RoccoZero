namespace O9K.AIO.Heroes.ArcWarden.Abilities
{
    using AIO.Abilities;

    using Core.Entities.Abilities.Base;
    using Core.Extensions;

    using Divine.Entity.Entities.Abilities.Components;

    using TargetManager;
    
    internal class DisableAbilityArcWarden : DisableAbility
    {
        public DisableAbilityArcWarden(ActiveAbility ability)
            : base(ability)
        {
        }

        public override bool ShouldCast(TargetManager targetManager)
        {
            var target = targetManager.Target;

            if (this.Ability.UnitTargetCast && !target.IsVisible)
            {
                return false;
            }

            if (this.Ability.BreaksLinkens && target.IsBlockingAbilities)
            {
                return false;
            }

            if (target.IsDarkPactProtected)
            {
                return false;
            }

            if (target.IsInvulnerable)
            {
                if (this.Disable.UnitTargetCast)
                {
                    return false;
                }

                if (!this.ChainStun(target, true))
                {
                    return false;
                }
            }

            if (this.Ability.GetDamage(target) < target.Health)
            {
                if (target.IsStunned)
                {
                    return this.ChainStun(target, false);
                }

                if (target.IsHexed)
                {
                    if (this.Disable.Id == AbilityId.item_bloodthorn &&
                        !target.HasModifier("modifier_bloodthorn_debuff"))
                    {
                        return true;
                    }

                    if (this.Disable.Id == AbilityId.item_orchid &&
                        !target.HasModifier("modifier_orchid_malevolence_debuff"))
                    {
                        return true;
                    }

                    return this.ChainStun(target, false);
                }

                if (target.IsSilenced)
                {
                    return this.ChainStun(target, false);
                }

                if (target.IsRooted)
                {
                    return !this.Disable.IsRoot() || this.ChainStun(target, false);
                }
            }

            if (target.IsRooted && !this.Ability.UnitTargetCast && target.GetImmobilityDuration() <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
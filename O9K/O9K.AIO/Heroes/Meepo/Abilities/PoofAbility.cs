namespace O9K.AIO.Heroes.Meepo.Abilities
{
    using System.Linq;

    using AIO.Abilities;

    using Core.Entities.Abilities.Base;
    using Core.Helpers;
    using Core.Managers.Entity;

    using TargetManager;

    internal class PoofAbility : NukeAbility
    {
        public PoofAbility(ActiveAbility ability) : base(ability)
        {
        }

        public override bool ShouldCast(TargetManager targetManager)
        {
            var target = targetManager.Target;

            if (Ability.UnitTargetCast && !target.IsVisible)
                return false;

            if (target.IsReflectingDamage)
                return false;

            if (Ability.BreaksLinkens && target.IsBlockingAbilities)
                return false;

            var damage = Ability.GetDamage(target);

            if (damage <= 0 && !target.HasModifier(breakShields))
                return false;

            if (target.IsInvulnerable)
            {
                if (Ability.UnitTargetCast)
                    return false;

                if (!ChainStun(target, true))
                    return false;
            }

            if (target.IsRooted && !Ability.UnitTargetCast && target.GetImmobilityDuration() <= 0)
                return false;

            if (!EntityManager9.AllyHeroes.Where(x => x.Name == EntityManager9.Owner.HeroName).Any(x => x.Distance(targetManager.Target) < Ability.Radius))
                return false;

            return true;
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            if (!Ability.UseAbility(targetManager.Target.Position))
                return false;

            var delay = Ability.GetCastDelay(targetManager.Target);

            comboSleeper.Sleep(delay);
            Sleeper.Sleep(delay + 0.5f);
            OrbwalkSleeper.Sleep(delay);

            return true;
        }
    }
}
namespace O9K.AIO.Heroes.StormSpirit.Units
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using AIO.Abilities;
    using AIO.Abilities.Items;
    using AIO.Modes.Combo;

    using Base;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Extensions;
    using Core.Helpers;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Extensions;
    using Divine.Projectile;

    using TargetManager;

    [UnitName(nameof(HeroId.npc_dota_hero_storm_spirit))]
    internal class StormSpirit : ControllableUnit //, IDisposable
    {
        private readonly Sleeper overloadSleeper = new();

        private BallLightning ball;

        private DisableAbility bloodthorn;

        private NukeAbility dagon;

        private DisableAbility hex;

        private Nullifier nullifier;

        private DisableAbility orchid;

        private BuffAbility overload;

        private NukeAbility remnant;

        private DebuffAbility shiva;

        private DisableAbility vortex;

        public StormSpirit(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.storm_spirit_static_remnant, x => remnant = new NukeAbility(x) },
                { AbilityId.storm_spirit_electric_vortex, x => vortex = new DisableAbility(x) },
                { AbilityId.storm_spirit_ball_lightning, x => ball = new BallLightning(x) },
                { AbilityId.storm_spirit_overload, x => overload = new BuffAbility(x) },

                { AbilityId.item_orchid, x => orchid = new DisableAbility(x) },
                { AbilityId.item_sheepstick, x => hex = new DisableAbility(x) },
                { AbilityId.item_bloodthorn, x => bloodthorn = new Bloodthorn(x) },
                { AbilityId.item_shivas_guard, x => shiva = new DebuffAbility(x) },
                { AbilityId.item_nullifier, x => nullifier = new Nullifier(x) },
                { AbilityId.item_dagon_5, x => dagon = new NukeAbility(x) }
            };

            MoveComboAbilities.Add(AbilityId.storm_spirit_ball_lightning, _ => ball);
        }

        public void ChargeOverload()
        {
            if (!IsValid || overloadSleeper)
            {
                return;
            }

            if (Owner.HasModifier("modifier_storm_spirit_overload"))
            {
                return;
            }

            var ult = ball?.Ability;

            if (ult?.CanBeCasted() != true)
            {
                return;
            }

            ult.UseAbility(Owner.IsMoving ? Owner.InFront(100) : Owner.InFront(25));
            overloadSleeper.Sleep(1);
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);
            var target = targetManager.Target;

            if (abilityHelper.UseAbility(hex))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(vortex) && (target.CanBecomeMagicImmune || target.CanBecomeInvisible))
            {
                if (abilityHelper.UseAbility(vortex))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbility(bloodthorn))
            {
                return true;
            }

            if (abilityHelper.UseAbility(orchid))
            {
                return true;
            }

            if (abilityHelper.UseAbility(nullifier))
            {
                return true;
            }

            if (abilityHelper.UseAbility(dagon))
            {
                return true;
            }

            if (abilityHelper.UseAbility(shiva))
            {
                return true;
            }

            var overloaded = Owner.CanAttack(target, 25) &&
                             Owner.HasModifier("modifier_storm_spirit_overload");

            var projectile = ProjectileManager.TrackingProjectiles.FirstOrDefault(x => x.Source?.Handle == Handle && x.Target?.Handle == target.Handle && x.IsAutoAttackProjectile());

            if (overloaded)
            {
                if (projectile == null)
                {
                    return false;
                }

                var distance = target.IsMoving && target.GetAngle(projectile.Position) > 1.5f ? 250 : 350;

                if (projectile.Position.Distance2D(projectile.TargetPosition) > distance)
                {
                    return false;
                }
            }
            else
            {
                if (projectile != null)
                {
                    var overload = Owner.Abilities.FirstOrDefault(x => x.Id == AbilityId.storm_spirit_overload);

                    if (overload != null)
                    {
                        var attackDamage = Owner.GetAttackDamage(target);
                        var overloadDamage = overload.GetDamage(target);
                        var health = target.Health;

                        if (attackDamage < health && attackDamage + overloadDamage > health)
                        {
                            if (abilityHelper.CanBeCasted(remnant, false, false) && abilityHelper.ForceUseAbility(remnant, true))
                            {
                                return true;
                            }

                            if (abilityHelper.CanBeCasted(ball, false, false))
                            {
                                var distance = projectile.Position.Distance2D(projectile.TargetPosition);
                                var time = distance / projectile.Speed;

                                if (time > ball.Ability.CastPoint && abilityHelper.ForceUseAbility(ball, true))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            if (abilityHelper.UseAbility(vortex))
            {
                ComboSleeper.ExtendSleep(0.1f);
                remnant?.Sleeper.Sleep(1f);
                ball?.Sleeper.Sleep(1f);

                return true;
            }

            if (abilityHelper.UseAbility(remnant))
            {
                ComboSleeper.ExtendSleep(0.1f);
                ball?.Sleeper.Sleep(1f);

                return true;
            }

            if (abilityHelper.UseAbilityIfCondition(ball, remnant, vortex))
            {
                ComboSleeper.ExtendSleep(0.3f);

                return true;
            }

            if (abilityHelper.UseAbility(this.overload, Owner.GetAttackRange()))
            {
                return true;
            }

            return false;
        }

        protected override bool MoveComboUseBlinks(AbilityHelper abilityHelper)
        {
            if (base.MoveComboUseBlinks(abilityHelper))
            {
                return true;
            }

            if (abilityHelper.UseMoveAbility(ball))
            {
                return true;
            }

            return false;
        }
    }
}
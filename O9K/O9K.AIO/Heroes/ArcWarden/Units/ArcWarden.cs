namespace O9K.AIO.Heroes.ArcWarden.Units
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
    using Core.Managers.Entity;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Components;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Extensions;
    using Divine.Order;

    using Draw;

    using TargetManager;

    using Utils;

    [UnitName(nameof(HeroId.npc_dota_hero_arc_warden))]
    internal class ArcWarden : ControllableUnit, IPushUnit
    {
        public static int TpCount;

        private readonly LaneHelper laneHelper = new();

        private readonly Sleeper moveSleeper = new();

        private DisableAbilityArcWarden abyssal;

        private DisableAbilityArcWarden atos;

        private BlinkAbility blink;

        private DisableAbilityArcWarden bloodthorn;

        private NukeAbility dagon;

        private DebuffAbility diffusal;

        private EtherealBlade ethereal;

        private DebuffAbility flux;

        private ForceStaff force;

        private DisableAbilityArcWarden gungir;

        private DisableAbilityArcWarden hex;

        private MagneticFieldAbility magneticFieldAbility;

        private BuffAbility manta;

        private ShieldAbility mjollnir;

        private Nullifier nullifier;

        private DisableAbilityArcWarden orchid;

        private HurricanePike pike;

        private BuffAbility shadow;

        private BuffAbility silver;

        private NukeAbility spark;

        private BuffAbility tempestDouble;

        private TravelBoots tpScroll;

        public ArcWarden(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.arc_warden_spark_wraith, x => spark = new NukeAbility(x) },
                { AbilityId.arc_warden_flux, x => flux = new DebuffAbility(x) },
                { AbilityId.arc_warden_magnetic_field, x => magneticFieldAbility = new MagneticFieldAbility(x) },
                { AbilityId.arc_warden_tempest_double, x => tempestDouble = new BuffAbility(x) },

                { AbilityId.item_rod_of_atos, x => atos = new DisableAbilityArcWarden(x) },
                { AbilityId.item_gungir, x => gungir = new DisableAbilityArcWarden(x) },
                { AbilityId.item_diffusal_blade, x => diffusal = new DebuffAbility(x) },
                { AbilityId.item_abyssal_blade, x => abyssal = new DisableAbilityArcWarden(x) },
                { AbilityId.item_manta, x => manta = new BuffAbility(x) },
                { AbilityId.item_bloodthorn, x => bloodthorn = new DisableAbilityArcWarden(x) },
                { AbilityId.item_orchid, x => orchid = new DisableAbilityArcWarden(x) },
                { AbilityId.item_nullifier, x => nullifier = new Nullifier(x) },
                { AbilityId.item_sheepstick, x => hex = new DisableAbilityArcWarden(x) },
                { AbilityId.item_mjollnir, x => mjollnir = new ShieldAbility(x) },

                { AbilityId.item_blink, x => blink = new BlinkDaggerArcWarden(x) },
                { AbilityId.item_swift_blink, x => blink = new BlinkDaggerArcWarden(x) },
                { AbilityId.item_arcane_blink, x => blink = new BlinkDaggerArcWarden(x) },
                { AbilityId.item_overwhelming_blink, x => blink = new BlinkDaggerArcWarden(x) },
                { AbilityId.item_hurricane_pike, x => pike = new HurricanePike(x) },
                { AbilityId.item_force_staff, x => force = new ForceStaff(x) },

                { AbilityId.item_ethereal_blade, x => ethereal = new EtherealBlade(x) },
                { AbilityId.item_dagon_5, x => dagon = new NukeAbility(x) },

                { AbilityId.item_silver_edge, x => silver = new BuffAbility(x) },
                { AbilityId.item_invis_sword, x => shadow = new BuffAbility(x) },

                { AbilityId.item_tpscroll, x => tpScroll = new TravelBoots(x) }
            };
        }

        public bool PushCombo()
        {
            if (OrderManager.Orders.Count() != 0)
            {
                return false;
            }

            if (Owner.IsChanneling)
            {
                return false;
            }

            var allyCreeps = EntityManager9.AllyUnits.Where(
                x => x.IsCreep && x.IsValid && x.IsAlive).ToList();

            var enemyCreeps =  EntityManager9.EnemyUnits.Where(
                x => x.IsCreep && x.IsValid && x.IsAlive).ToList();

            if (TpCount > 0 && TravelTpToCreeps(enemyCreeps, allyCreeps))
            {
                return true;
            }

            if (UseMjolnir(allyCreeps))
            {
                return true;
            }

            var nearestTower =
                EntityManager9.EnemyUnits
                    .Where(x => x.BaseUnit.NetworkName == ClassId.CDOTA_BaseNPC_Tower.ToString() && x.IsValid && x.IsAlive)
                    .OrderBy(y => Owner.Distance(y))
                    .FirstOrDefault();

            if (nearestTower == null)
            {
                nearestTower = EntityManager9.EnemyUnits.Where(x => x.IsBuilding && x.IsValid && x.IsAlive && x.CanDie).OrderBy(y => Owner.Distance(y))
                    .FirstOrDefault();
            }

            var currentLane = laneHelper.GetCurrentLane(Owner);
            var attackPoint = laneHelper.GetClosestAttackPoint(Owner, currentLane);

            if (UseSpark(enemyCreeps))
            {
                return true;
            }

            if (nearestTower?.Distance(Owner) <= 900)
            {
                if (UseMagneticFieldNearTower(nearestTower))
                {
                    return true;
                }

                if (PushCommands.AttackTower(Owner, nearestTower))
                {
                    return true;
                }
            }

            if (UseMagneticFieldNearCreeps(enemyCreeps))
            {
                return true;
            }

            if (PushCommands.AttackNextPoint(Owner, attackPoint))
            {
                return true;
            }

            return true;
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);
            var isMainHero = Owner == EntityManager9.Owner;

            if (OrderManager.Orders.Count() != 0)
            {
                return false;
            }

            if (abilityHelper.UseAbility(blink))
            {
                return true;
            }

            if (!isMainHero && !Owner.HasModifier(magneticFieldAbility.Shield.ShieldModifierName) &&
                abilityHelper.UseAbility(force, 500, 300))
            {
                return true;
            }

            if (!isMainHero && !Owner.HasModifier(magneticFieldAbility.Shield.ShieldModifierName) &&
                abilityHelper.UseAbility(force, 500, 300))
            {
                return true;
            }

            if ((!Owner.HasModifier(magneticFieldAbility.Shield.ShieldModifierName) || Owner.Health < 1000) &&
                isMainHero && abilityHelper.CanBeCasted(pike) && !MoveSleeper.IsSleeping)
            {
                if (pike.UseAbilityOnTarget(targetManager, ComboSleeper))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbility(atos))
            {
                return true;
            }

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWardenMagneticRootDisable") &&
                abilityHelper.UseAbility(gungir))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWardenMagneticRootDisable", 2000);

                return true;
            }

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWardenMagneticRootDisable") &&
                abilityHelper.UseAbility(abyssal))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWardenMagneticRootDisable", 2000);

                return true;
            }

            if (abilityHelper.UseAbility(hex))
            {
                return true;
            }

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWardenMagneticNullifier") &&
                abilityHelper.UseAbility(nullifier))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWardenMagneticNullifier", 2000);

                return true;
            }

            if (abilityHelper.UseAbility(bloodthorn))
            {
                return true;
            }

            if (abilityHelper.UseAbility(orchid))
            {
                return true;
            }

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWardenMagneticSilverEdge") &&
                abilityHelper.UseAbility(silver))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWardenMagneticSilverEdge", 5000);
                ComboSleeper.Sleep(0.5f);

                return true;
            }

            if (!isMainHero && abilityHelper.UseAbility(shadow))
            {
                ComboSleeper.Sleep(0.5f);

                return true;
            }

            if (abilityHelper.UseAbility(flux))
            {
                return true;
            }

            if (abilityHelper.UseAbility(tempestDouble))
            {
                return true;
            }

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWardenMagneticField") &&
                abilityHelper.UseAbility(magneticFieldAbility))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWardenMagneticField", 550);

                return true;
            }

            if (abilityHelper.UseAbility(diffusal))
            {
                return true;
            }
            // fast nukes section before sparks

            if (abilityHelper.UseAbility(ethereal))
            {
                return true;
            }

            if (abilityHelper.UseAbility(dagon))
            {
                return true;
            }

            // end of fast nukes

            if (abilityHelper.UseAbility(spark))
            {
                return true;
            }

            if (abilityHelper.UseAbility(manta, Owner.GetAttackRange() + 20))
            {
                return true;
            }

            if (abilityHelper.UseAbility(mjollnir, 600))
            {
                return true;
            }

            return false;
        }

        private bool UseMagneticFieldNearCreeps(List<Unit9> enemyCreeps)
        {
            if (enemyCreeps.Count(x => x.Distance(Owner) < Owner.GetAttackRange()) >= 4 && magneticFieldAbility.Ability.CanBeCasted())
            {
                magneticFieldAbility.Ability.UseAbility(Owner.Position);

                return true;
            }

            return false;
        }

        private bool UseMagneticFieldNearTower(Unit9 unit)
        {
            if (magneticFieldAbility.Ability.CanBeCasted())
            {
                magneticFieldAbility.Ability.UseAbility(Owner.Position.Extend2D(unit.Position, 100));

                return true;
            }

            return false;
        }

        private bool UseSpark(List<Unit9> enemyCreeps)
        {
            if (enemyCreeps.Count(x => x.Distance(Owner) <= 1000) > 2)
            {
                if (spark.Ability.CanBeCasted())
                {
                    var enemyCreep = enemyCreeps.FirstOrDefault(unit => unit.Distance(Owner) <= 1000 && unit.IsRanged) ??  enemyCreeps.FirstOrDefault(unit => unit.Distance(Owner) <= 1000);

                    if (enemyCreep != null)
                    {
                        spark.Ability.UseAbility(enemyCreep.Position);
                    }

                    Divine.Helpers.MultiSleeper<string>.Sleep("ArcWarden.PushCombo.Attack" + Owner.Handle, 1000);

                    return true;
                }
            }

            return false;
        }

        private bool UseMjolnir(List<Unit9> allyCreeps)
        {
            if (mjollnir != null && mjollnir.Ability.CanBeCasted() && Owner.GetModifier("modifier_kill").RemainingTime < 3)
            {
                var allyCreep = allyCreeps.FirstOrDefault(x => !x.IsRanged && x.HealthPercentage > 65 && x.Distance(Owner) < mjollnir.Ability.CastRange)
                                ?? allyCreeps.FirstOrDefault(x => x.Distance(Owner) < mjollnir.Ability.CastRange);

                if (allyCreep != null)
                {
                    mjollnir.Ability.UseAbility(allyCreep);

                    return true;
                }
            }

            return false;
        }

        private bool TravelTpToCreeps(List<Unit9> enemyCreeps, List<Unit9> allyCreeps)
        {
            if (Owner.IsHero && Owner.GetModifier("modifier_kill").RemainingTime >= 10 && tpScroll.Ability.CanBeCasted())
            {
                var chosenLane = ArcWardenDrawPanel.lane;

                if (chosenLane == Lane.AUTO)
                {
                    chosenLane = laneHelper.GetCurrentLane(
                        allyCreeps.Where(x => laneHelper.GetCurrentLane(x) != laneHelper.GetCurrentLane(Owner))
                            .OrderByDescending(x => Owner.Distance(x))
                            .FirstOrDefault());
                }

                var finalPos = laneHelper.GetPath(chosenLane).Last();

                var allyCreepsOrdered = allyCreeps.Where(
                        x => laneHelper.GetCurrentLane(x) == chosenLane &&
                             x.HealthPercentage > 75)
                    .OrderBy(y => y.Distance(finalPos));

                var ally =
                    allyCreepsOrdered
                        .Where(x => allyCreeps.Any(unit => unit.Distance(x) < 500 && unit.Handle != x.Handle)
                                    || !enemyCreeps.Any(enemyCreep => enemyCreep.Distance(x) < 1000))
                        .FirstOrDefault();

                var allyTwr =
                    EntityManager9.AllyUnits.Where(
                            x => x.IsTower && x.IsValid && x.IsAlive &&
                                 laneHelper.GetCurrentLane(x) ==  chosenLane &&
                                 x.HealthPercentage > 0.1)
                        .OrderBy(y => y.Distance(finalPos))
                        .FirstOrDefault();

                Unit9 tpTarget = null;

                if (ally != null && allyTwr != null)
                {
                    var dist1 = finalPos.Distance2D(ally.Position);
                    var dist2 = finalPos.Distance2D(allyTwr.Position);

                    if (dist1 > dist2)
                    {
                        tpTarget = allyTwr;
                    }
                    else
                    {
                        tpTarget = ally;
                    }
                }

                if (tpTarget != null && tpTarget.Distance(Owner) > 1500)
                {
                    var point = laneHelper.GetPath(chosenLane).Last();
                    var distance1 = point.Distance2D(tpTarget.Position);
                    var distance2 = point.Distance2D(Owner.Position);

                    if (distance1 < distance2 || laneHelper.GetCurrentLane(Owner) != chosenLane)
                    {
                        if (UseTp(tpTarget))
                        {
                            TpCount--;

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool UseTp(Unit9 unit)
        {
            if (Owner.Distance(unit) < 3000)
            {
                return false;
            }

            return  tpScroll.Ability.UseAbility(unit);
        }
    }
}
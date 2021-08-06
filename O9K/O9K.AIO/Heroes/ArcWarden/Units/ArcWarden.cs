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
    using Divine.Numerics;
    using Divine.Order;

    using Draw;

    using TargetManager;

    using Utils;

    [UnitName(nameof(HeroId.npc_dota_hero_arc_warden))]
    internal class ArcWarden : ControllableUnit
    {
        private readonly Sleeper moveSleeper = new Sleeper();

        private readonly LaneHelper laneHelper = new LaneHelper();

        private DisableAbilityArcWarden abyssal;

        private DisableAbilityArcWarden bloodthorn;

        private DebuffAbility diffusal;

        private NukeAbility spark;

        private DebuffAbility flux;

        private MagneticFieldAbility magneticFieldAbility;

        private BuffAbility tempestDouble;

        private BuffAbility manta;

        private Nullifier nullifier;

        private DisableAbilityArcWarden hex;

        private DisableAbilityArcWarden orchid;

        private BlinkAbility blink;

        private HurricanePike pike;

        private ForceStaff force;

        private ShieldAbility mjollnir;

        private DisableAbilityArcWarden atos;

        private DisableAbilityArcWarden gungir;

        private EtherealBlade ethereal;

        private NukeAbility dagon;

        private BuffAbility shadow;

        private BuffAbility silver;

        private TravelBoots tpScroll;

        public ArcWarden(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.arc_warden_spark_wraith, x => this.spark = new NukeAbility(x) },
                { AbilityId.arc_warden_flux, x => this.flux = new DebuffAbility(x) },
                { AbilityId.arc_warden_magnetic_field, x => this.magneticFieldAbility = new MagneticFieldAbility(x) },
                { AbilityId.arc_warden_tempest_double, x => this.tempestDouble = new BuffAbility(x) },

                { AbilityId.item_rod_of_atos, x => this.atos = new DisableAbilityArcWarden(x) },
                { AbilityId.item_gungir, x => this.gungir = new DisableAbilityArcWarden(x) },
                { AbilityId.item_diffusal_blade, x => this.diffusal = new DebuffAbility(x) },
                { AbilityId.item_abyssal_blade, x => this.abyssal = new DisableAbilityArcWarden(x) },
                { AbilityId.item_manta, x => this.manta = new BuffAbility(x) },
                { AbilityId.item_bloodthorn, x => this.bloodthorn = new DisableAbilityArcWarden(x) },
                { AbilityId.item_orchid, x => this.orchid = new DisableAbilityArcWarden(x) },
                { AbilityId.item_nullifier, x => this.nullifier = new Nullifier(x) },
                { AbilityId.item_sheepstick, x => this.hex = new DisableAbilityArcWarden(x) },
                { AbilityId.item_mjollnir, x => this.mjollnir = new ShieldAbility(x) },

                { AbilityId.item_blink, x => this.blink = new BlinkDaggerArcWarden(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkDaggerArcWarden(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkDaggerArcWarden(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkDaggerArcWarden(x) },
                { AbilityId.item_hurricane_pike, x => this.pike = new HurricanePike(x) },
                { AbilityId.item_force_staff, x => this.force = new ForceStaff(x) },

                { AbilityId.item_ethereal_blade, x => this.ethereal = new EtherealBlade(x) },
                { AbilityId.item_dagon_5, x => this.dagon = new NukeAbility(x) },

                { AbilityId.item_silver_edge, x => this.silver = new BuffAbility(x) },
                { AbilityId.item_invis_sword, x => this.shadow = new BuffAbility(x) },

                { AbilityId.item_tpscroll, x => this.tpScroll = new TravelBoots(x) },
            };
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);
            var isMainHero = (this.Owner == EntityManager9.Owner);

            if (OrderManager.Orders.Count() != 0)
            {
                return false;
            }

            if (abilityHelper.UseAbility(this.blink))
            {
                return true;
            }

            if (!isMainHero && !this.Owner.HasModifier(magneticFieldAbility.Shield.ShieldModifierName) &&
                abilityHelper.UseAbility(this.force, 500, 300))
            {
                return true;
            }

            if (!isMainHero && !this.Owner.HasModifier(magneticFieldAbility.Shield.ShieldModifierName) &&
                abilityHelper.UseAbility(this.force, 500, 300))
            {
                return true;
            }

            if ((!this.Owner.HasModifier(magneticFieldAbility.Shield.ShieldModifierName) || this.Owner.Health < 1000) &&
                isMainHero && abilityHelper.CanBeCasted(this.pike) && !this.MoveSleeper.IsSleeping)
            {
                if (this.pike.UseAbilityOnTarget(targetManager, this.ComboSleeper))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbility(this.atos))
            {
                return true;
            }

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWardenMagneticRootDisable") &&
                abilityHelper.UseAbility(this.gungir))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWardenMagneticRootDisable", 2000);

                return true;
            }

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWardenMagneticRootDisable") &&
                abilityHelper.UseAbility(this.abyssal))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWardenMagneticRootDisable", 2000);

                return true;
            }

            if (abilityHelper.UseAbility(hex))
            {
                return true;
            }

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWardenMagneticNullifier") &&
                abilityHelper.UseAbility(this.nullifier))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWardenMagneticNullifier", 2000);

                return true;
            }

            if (abilityHelper.UseAbility(this.bloodthorn))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.orchid))
            {
                return true;
            }

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWardenMagneticSilverEdge") &&
                abilityHelper.UseAbility(this.silver))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWardenMagneticSilverEdge", 5000);
                ComboSleeper.Sleep(0.5f);

                return true;
            }

            if (!isMainHero && abilityHelper.UseAbility(this.shadow))
            {
                ComboSleeper.Sleep(0.5f);

                return true;
            }

            if (abilityHelper.UseAbility(this.flux))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.tempestDouble))
            {
                return true;
            }

            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWardenMagneticField") &&
                abilityHelper.UseAbility(this.magneticFieldAbility))
            {
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWardenMagneticField", 550);

                return true;
            }

            if (abilityHelper.UseAbility(this.diffusal))
            {
                return true;
            }
            // fast nukes section before sparks

            if (abilityHelper.UseAbility(this.ethereal))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.dagon))
            {
                return true;
            }

            // end of fast nukes

            if (abilityHelper.UseAbility(this.spark))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.manta, this.Owner.GetAttackRange() + 20))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.mjollnir, 600))
            {
                return true;
            }

            return false;
        }

        protected override bool Attack(Unit9 target, ComboModeMenu comboMenu)
        {
            if (this.Owner.Distance(target) > this.Owner.GetAttackRange(target, 200))
            {
                if (this.Owner.BaseUnit.Attack(target.BaseUnit))
                {
                    this.AttackSleeper.Sleep(0.5f);
                    this.MoveSleeper.Sleep(0.5f);

                    return true;
                }
            }

            return base.Attack(target, comboMenu);
        }

        protected override bool MoveComboUseBlinks(AbilityHelper abilityHelper)
        {
            if (this.moveSleeper)
            {
                return false;
            }

            if (base.MoveComboUseBlinks(abilityHelper))
            {
                return true;
            }

            if (this.Owner.IsAttacking)
            {
                this.OrbwalkSleeper.Reset();
                this.MoveSleeper.Reset();

                return true;
            }

            return false;
        }

        public bool PushCombo()
        {
            if (OrderManager.Orders.Count() != 0)
            {
                return false;
            }

            if (this.Owner.IsChanneling)
            {
                return false;
            }

            var allyCreeps = EntityManager9.AllyUnits.Where(x => x.BaseUnit.NetworkName == ClassId.CDOTA_BaseNPC_Creep_Lane.ToString() && x.IsValid && x.IsAlive).ToList();
            var enemyCreeps =  EntityManager9.EnemyUnits.Where(x => x.BaseUnit.NetworkName == ClassId.CDOTA_BaseNPC_Creep_Lane.ToString() && x.IsValid && x.IsAlive).ToList();

            var creepWithEnemy = allyCreeps.Where(
                x => x.HealthPercentage > 65 &&
                     enemyCreeps.Any(y => y.Distance(x) <= 1000)).OrderByDescending(x => x.Distance(this.Owner)).FirstOrDefault();

            if (TravelTpToCreeps(enemyCreeps, allyCreeps))
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
                    .OrderBy(y => this.Owner.Distance(y))
                    .FirstOrDefault();

            if (nearestTower == null)
            {
                nearestTower = EntityManager9.EnemyUnits.Where(x => x.IsBuilding && x.IsValid && x.IsAlive && x.CanDie).OrderBy(y => this.Owner.Distance(y))
                    .FirstOrDefault();
            }

            var currentLane = laneHelper.GetCurrentLane(this.Owner);
            var attackPoint = laneHelper.GetClosestAttackPoint(this.Owner, currentLane);

            if (UseSpark(enemyCreeps))
            {
                return true;
            }

            if (this.Owner.Distance(nearestTower) <= 900)
            {
                if (UseMagneticFieldNearTower(nearestTower))
                {
                    return true;
                }

                if (AttackTower(nearestTower))
                {
                    return true;
                }
            }

            if (UseMagneticFieldNearCreeps(enemyCreeps))
            {
                return true;
            }

            if (AttackNextPoint(attackPoint))
            {
                return true;
            }

            return true;
        }

        private bool UseMagneticFieldNearCreeps(List<Unit9> enemyCreeps)
        {
            if (enemyCreeps.Count(x => x.Distance(this.Owner) < this.Owner.GetAttackRange()) >= 4 && this.magneticFieldAbility.Ability.CanBeCasted())
            {
                this.magneticFieldAbility.Ability.UseAbility(this.Owner.Position);

                return true;
            }

            return false;
        }

        private bool AttackNextPoint(Vector3 attackPoint)
        {
            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWarden.PushCombo.Attack" + this.Owner.Handle))
            {
                this.Owner.BaseUnit.Attack(attackPoint);
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWarden.PushCombo.Attack" + this.Owner.Handle, 1500);

                return true;
            }

            return false;
        }

        private bool AttackTower(Unit9 nearestTower)
        {
            if (!this.Owner.IsAttacking && !Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWarden.PushCombo.Attack" + this.Owner.Handle) && !nearestTower.IsInvulnerable)
            {
                this.Owner.Attack(nearestTower);
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWarden.PushCombo.Attack" + this.Owner.Handle, 400);

                return true;
            }

            return false;
        }

        private bool UseMagneticFieldNearTower(Unit9? unit)
        {
            if (this.magneticFieldAbility.Ability.CanBeCasted())
            {
                this.magneticFieldAbility.Ability.UseAbility(this.Owner.Position.Extend2D(unit.Position, 100));

                return true;
            }

            return false;
        }

        private bool UseSpark(List<Unit9> enemyCreeps)
        {
            if (enemyCreeps.Count(x => x.Distance(this.Owner) <= 800) > 2 && this.Owner.IsHero)
            {
                if (this.spark.Ability.CanBeCasted())
                {
                    var enemyCreep = enemyCreeps.FirstOrDefault(unit => unit.Distance(this.Owner) <= 1000 && unit.IsRanged) ??  enemyCreeps.FirstOrDefault(unit => unit.Distance(this.Owner) <= 1000);
                    this.spark.Ability.UseAbility(enemyCreep);

                    return true;
                }
            }

            return false;
        }

        private bool UseMjolnir(List<Unit9> allyCreeps)
        {
            if (mjollnir != null && mjollnir.Ability.CanBeCasted() && this.Owner.GetModifier("modifier_kill").RemainingTime < 3)
            {
                var allyCreep = allyCreeps.FirstOrDefault(x => !x.IsRanged && x.HealthPercentage > 65 && x.Distance(this.Owner) < this.mjollnir.Ability.CastRange)
                                ?? allyCreeps.FirstOrDefault(x => x.Distance(this.Owner) < this.mjollnir.Ability.CastRange);

                if (allyCreep != null)
                {
                    this.mjollnir.Ability.UseAbility(allyCreep);

                    return true;
                }
            }

            return false;
        }

        private bool TravelTpToCreeps(List<Unit9> enemyCreeps, List<Unit9> allyCreeps)
        {
            Unit9? creepWithEnemy;

            if (this.Owner.IsHero && this.Owner.GetModifier("modifier_kill").RemainingTime >= 10 && tpScroll.Ability.CanBeCasted())
            {
                if (tpScroll != null && (laneHelper.GetCurrentLane(this.Owner) != AutoPushingPanelTest.lane || !enemyCreeps.Any(x => x.Distance(this.Owner) <= 1000)) &&
                    !this.Owner.IsChanneling)
                {
                    if (AutoPushingPanelTest.lane == Lane.AUTO)
                    {
                        creepWithEnemy = allyCreeps.Where(
                            x => x.HealthPercentage > 65 &&
                                 enemyCreeps.Any(y => y.Distance(x) <= 1000)).OrderByDescending(x => x.Distance(this.Owner)).FirstOrDefault();
                    }
                    else
                    {
                        creepWithEnemy = allyCreeps.Where(
                            x => x.HealthPercentage > 65 &&
                                 enemyCreeps.Any(y => y.Distance(x) <= 1000 && laneHelper.GetCurrentLane(y) == AutoPushingPanelTest.lane)).FirstOrDefault();
                    }

                    if (creepWithEnemy == null)
                    {
                        return  false;
                    }

                    tpScroll.Ability.UseAbility(creepWithEnemy);

                    return true;
                }
            }

            return false;
        }
    }
}
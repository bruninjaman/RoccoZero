namespace O9K.AIO.Heroes.ArcWarden.Units;

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

    public bool PushCombo()
    {
        if (OrderManager.Orders.Any())
        {
            return false;
        }

        if (this.Owner.IsChanneling)
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
                          .MinBy(y => this.Owner.Distance(y));

        if (nearestTower == null)
        {
            nearestTower = EntityManager9.EnemyUnits.Where(x => x.IsBuilding && x.IsValid && x.IsAlive && x.CanDie).MinBy(y => this.Owner.Distance(y));
        }

        var currentLane = this.laneHelper.GetCurrentLane(this.Owner);
        var attackPoint = this.laneHelper.GetClosestAttackPoint(this.Owner, currentLane);

        if (UseSpark(enemyCreeps))
        {
            return true;
        }

        if (nearestTower?.Distance(this.Owner) <= 900)
        {
            if (UseMagneticFieldNearTower(nearestTower))
            {
                return true;
            }

            if (PushCommands.AttackTower(this.Owner, nearestTower))
            {
                return true;
            }
        }

        if (UseMagneticFieldNearCreeps(enemyCreeps))
        {
            return true;
        }

        if (PushCommands.AttackNextPoint(this.Owner, attackPoint))
        {
            return true;
        }

        return true;
    }

    public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
    {
        var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);
        var isMainHero = this.Owner == EntityManager9.Owner;

        if (OrderManager.Orders.Count() != 0)
        {
            return false;
        }

        if (abilityHelper.UseAbility(this.blink))
        {
            return true;
        }

        if (!isMainHero && !this.Owner.HasModifier(this.magneticFieldAbility.Shield.ShieldModifierName) &&
            abilityHelper.UseAbility(this.force, 500, 300))
        {
            return true;
        }

        if (!isMainHero && !this.Owner.HasModifier(this.magneticFieldAbility.Shield.ShieldModifierName) &&
            abilityHelper.UseAbility(this.force, 500, 300))
        {
            return true;
        }

        if ((!this.Owner.HasModifier(this.magneticFieldAbility.Shield.ShieldModifierName) || this.Owner.Health < 1000) &&
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

        if (abilityHelper.UseAbility(this.hex))
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
            this.ComboSleeper.Sleep(0.5f);

            return true;
        }

        if (!isMainHero && abilityHelper.UseAbility(this.shadow))
        {
            this.ComboSleeper.Sleep(0.5f);

            return true;
        }
        
        if (abilityHelper.UseAbility(this.flux))
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

    private bool UseMagneticFieldNearCreeps(List<Unit9> enemyCreeps)
    {
        if (enemyCreeps.Count(x => x.Distance(this.Owner) < this.Owner.GetAttackRange()) >= 4 && this.magneticFieldAbility.Ability.CanBeCasted())
        {
            this.magneticFieldAbility.Ability.UseAbility(this.Owner.Position);

            return true;
        }

        return false;
    }

    private bool UseMagneticFieldNearTower(Unit9 unit)
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
        if (enemyCreeps.Count(x => x.Distance(this.Owner) <= 1000) > 2)
        {
            if (this.spark.Ability.CanBeCasted())
            {
                var enemyCreep = enemyCreeps.FirstOrDefault(unit => unit.Distance(this.Owner) <= 1000 && unit.IsRanged) ?? enemyCreeps.FirstOrDefault(unit => unit.Distance(this.Owner) <= 1000);

                if (enemyCreep != null)
                {
                    this.spark.Ability.UseAbility(enemyCreep.Position);
                }

                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWarden.PushCombo.Attack" + this.Owner.Handle, 1000);

                return true;
            }
        }

        return false;
    }

        private bool UseMjolnir(List<Unit9> allyCreeps)
    {
        if (this.mjollnir != null && this.mjollnir.Ability.CanBeCasted() && this.Owner.GetModifier("modifier_kill").RemainingTime < 3)
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
        if (this.Owner.IsHero && this.Owner.GetModifier("modifier_kill").RemainingTime >= 10 && this.tpScroll.Ability.CanBeCasted())
        {
            var chosenLane = ArcWardenPanel.lane;

            if (chosenLane == Lane.AUTO)
            {
                var creep = allyCreeps.Where(x => this.laneHelper.GetCurrentLane(x) != this.laneHelper.GetCurrentLane(this.Owner))
                                      .OrderByDescending(x => this.Owner.Distance(x))
                                      .FirstOrDefault();

                if (creep != null)
                {
                    chosenLane = this.laneHelper.GetCurrentLane(creep);
                }
            }

            var finalPos = this.laneHelper.GetPath(chosenLane).Last();

            var allyCreepsOrdered = allyCreeps.Where(x => this.laneHelper.GetCurrentLane(x) == chosenLane && x.HealthPercentage > 75)
                                              .OrderBy(y => y.Distance(finalPos));

            var ally =
                allyCreepsOrdered
                    .Where(x => (allyCreeps.Any(unit => unit.Distance(x) < 500 && unit.Handle != x.Handle)
                                 || !enemyCreeps.Any(enemyCreep => enemyCreep.Distance(x) < 1000))
                                && !EntityManager9.EnemyHeroes.Any(enemyHero => x.Distance(enemyHero) < 3000))
                    .FirstOrDefault();

            var allyTwr =
                EntityManager9.AllyUnits.Where(
                                               x => x.IsTower && x.IsValid && x.IsAlive && this.laneHelper.GetCurrentLane(x) == chosenLane &&
                                                    x.HealthPercentage > 0.1)
                              .OrderBy(y => y.Distance(finalPos))
                              .FirstOrDefault();

            Unit9 tpTarget = null;

            if (ally != null && allyTwr != null)
            {
                float dist1 = finalPos.Distance2D(ally.Position);
                float dist2 = finalPos.Distance2D(allyTwr.Position);

                if (dist1 > dist2)
                {
                    tpTarget = allyTwr;
                }
                else
                {
                    tpTarget = ally;
                }
            }

            if (tpTarget != null && tpTarget.Distance(this.Owner) > 1500)
            {
                var point = this.laneHelper.GetPath(chosenLane).Last();
                float distance1 = point.Distance2D(tpTarget.Position);
                float distance2 = point.Distance2D(this.Owner.Position);

                if (distance1 < distance2 || this.laneHelper.GetCurrentLane(this.Owner) != chosenLane)
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
        if (this.Owner.Distance(unit) < 3000)
        {
            return false;
        }

        return this.tpScroll.Ability.UseAbility(unit);
    }
}
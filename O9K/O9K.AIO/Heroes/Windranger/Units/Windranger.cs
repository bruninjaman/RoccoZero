namespace O9K.AIO.Heroes.Windranger.Units
{
    using System;
    using System.Collections.Generic;

    using Abilities;

    using AIO.Abilities;
    using AIO.Abilities.Items;

    using Base;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Helpers;

    using Divine;
    using Divine.Camera;
    using Divine.Entity;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.GameConsole;

    using Divine.Input;
    using Divine.Log;
    using Divine.Map;

    using Divine.Modifier;
    using Divine.Numerics;
    using Divine.Orbwalker;
    using Divine.Order;
    using Divine.Particle;
    using Divine.Projectile;
    using Divine.Renderer;
    using Divine.Service;
    using Divine.Update;
    using Divine.Entity.Entities;
    using Divine.Entity.EventArgs;
    using Divine.Game.EventArgs;
    using Divine.GameConsole.Exceptions;
    using Divine.Input.EventArgs;
    using Divine.Map.Components;
    using Divine.Menu.Animations;
    using Divine.Menu.Components;

    using Divine.Menu.Helpers;

    using Divine.Menu.Styles;
    using Divine.Modifier.EventArgs;
    using Divine.Modifier.Modifiers;
    using Divine.Order.EventArgs;
    using Divine.Order.Orders;
    using Divine.Particle.Components;
    using Divine.Particle.EventArgs;
    using Divine.Particle.Particles;
    using Divine.Plugins.Humanizer;
    using Divine.Projectile.EventArgs;
    using Divine.Projectile.Projectiles;
    using Divine.Renderer.ValveTexture;
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Components;
    using Divine.Entity.Entities.EventArgs;
    using Divine.Entity.Entities.Exceptions;
    using Divine.Entity.Entities.PhysicalItems;
    using Divine.Entity.Entities.Players;
    using Divine.Entity.Entities.Runes;
    using Divine.Entity.Entities.Trees;
    using Divine.Entity.Entities.Units;
    using Divine.Modifier.Modifiers.Components;
    using Divine.Modifier.Modifiers.Exceptions;
    using Divine.Order.Orders.Components;
    using Divine.Particle.Particles.Exceptions;
    using Divine.Projectile.Projectiles.Components;
    using Divine.Projectile.Projectiles.Exceptions;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Abilities.Spells;
    using Divine.Entity.Entities.Players.Components;
    using Divine.Entity.Entities.Runes.Components;
    using Divine.Entity.Entities.Units.Buildings;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Entity.Entities.Units.Creeps;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Wards;
    using Divine.Entity.Entities.Abilities.Items.Components;
    using Divine.Entity.Entities.Abilities.Items.Neutrals;
    using Divine.Entity.Entities.Abilities.Spells.Abaddon;
    using Divine.Entity.Entities.Abilities.Spells.Components;
    using Divine.Entity.Entities.Units.Creeps.Neutrals;
    using Divine.Entity.Entities.Units.Heroes.Components;
using Modes.Combo;
using TargetManager;

    using BaseWindranger = Core.Entities.Heroes.Unique.Windranger;

    [UnitName(nameof(HeroId.npc_dota_hero_windrunner))]
    internal class Windranger : ControllableUnit, IDisposable
    {
        private readonly BaseWindranger windranger;

        private BlinkDaggerWindranger blink;

        private DisableAbility bloodthorn;

        private FocusFire focusFire;

        //private ForceStaff force;

        private DisableAbility hex;

        private Nullifier nullifier;

        private DisableAbility orchid;

        private SpeedBuffAbility phase;

        private Powershot powershot;

        private Shackleshot shackleshot;

        private Windrun windrun;

        private MoveBuffAbility windrunMove;

        public Windranger(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.windranger = owner as BaseWindranger;

            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                {
                    AbilityId.windrunner_shackleshot, x =>
                        {
                            this.shackleshot = new Shackleshot(x);
                            if (this.powershot != null)
                            {
                                this.powershot.Shackleshot = this.shackleshot;
                            }

                            return this.shackleshot;
                        }
                },
                {
                    AbilityId.windrunner_powershot, x =>
                        {
                            this.powershot = new Powershot(x);
                            if (this.shackleshot != null)
                            {
                                this.powershot.Shackleshot = this.shackleshot;
                            }

                            return this.powershot;
                        }
                },
                { AbilityId.windrunner_windrun, x => this.windrun = new Windrun(x) },
                { AbilityId.windrunner_focusfire, x => this.focusFire = new FocusFire(x) },

                { AbilityId.item_phase_boots, x => this.phase = new SpeedBuffAbility(x) },
                { AbilityId.item_blink, x => this.blink = new BlinkDaggerWindranger(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkDaggerWindranger(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkDaggerWindranger(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkDaggerWindranger(x) },
                //{ AbilityId.item_force_staff, x => this.force = new ForceStaff(x) },
                { AbilityId.item_orchid, x => this.orchid = new DisableAbility(x) },
                { AbilityId.item_bloodthorn, x => this.bloodthorn = new Bloodthorn(x) },
                { AbilityId.item_nullifier, x => this.nullifier = new Nullifier(x) },
                { AbilityId.item_sheepstick, x => this.hex = new DisableAbility(x) },
            };

            this.MoveComboAbilities.Add(AbilityId.windrunner_shackleshot, _ => this.shackleshot);
            this.MoveComboAbilities.Add(AbilityId.windrunner_windrun, x => this.windrunMove = new MoveBuffAbility(x));
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (this.powershot.CancelChanneling(targetManager))
            {
                this.ComboSleeper.Sleep(0.1f);
                return true;
            }

            if (abilityHelper.UseAbilityIfCondition(this.blink, this.shackleshot))
            {
                abilityHelper.ForceUseAbility(this.shackleshot);
                this.OrbwalkSleeper.Sleep(0.5f);
                return true;
            }

            if (!abilityHelper.CanBeCasted(this.shackleshot, false, false))
            {
                if (abilityHelper.UseAbility(this.blink, this.Owner.GetAttackRange(targetManager.Target) + 100, 400))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbility(this.hex))
            {
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

            if (abilityHelper.UseAbility(this.nullifier))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.shackleshot))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(this.shackleshot, false))
            {
                var castWindrun = abilityHelper.CanBeCasted(this.windrun, false, false);
                var movePosition = this.shackleshot.GetMovePosition(targetManager, comboModeMenu, castWindrun);
                //if ((movePosition.IsZero  || Owner.Distance(movePosition) > 300 || targetManager.Target.IsMoving) && abilityHelper.CanBeCasted(this.force, false, false))
                //{
                //    var forcePosition = this.shackleshot.GetForceStaffPosition(targetManager, this.force);
                //    if (!forcePosition.IsZero)
                //    {
                //        if (Owner.GetAngle(forcePosition) > 0.1)
                //        {
                //            this.Move(forcePosition);
                //            OrbwalkSleeper.Sleep(0.1f);
                //            this.ComboSleeper.Sleep(0.1f);
                //            return true;
                //        }

                //        abilityHelper.ForceUseAbility(this.force, true);
                //        return true;
                //    }
                //}

                if (!movePosition.IsZero && this.Move(movePosition))
                {
                    if ((this.Owner.Distance(movePosition) > 100 || targetManager.Target.IsMoving) && castWindrun)
                    {
                        abilityHelper.ForceUseAbility(this.windrun);
                    }

                    this.OrbwalkSleeper.Sleep(0.1f);
                    this.ComboSleeper.Sleep(0.1f);
                    return true;
                }
            }

            if (abilityHelper.UseAbilityIfCondition(this.powershot, this.shackleshot, this.blink))
            {
                this.ComboSleeper.ExtendSleep(0.2f);
                this.OrbwalkSleeper.ExtendSleep(0.2f);
                return true;
            }

            if (abilityHelper.UseAbilityIfCondition(this.focusFire, this.powershot))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.windrun))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.phase))
            {
                return true;
            }

            return false;
        }

        public virtual bool DirectionalMove()
        {
            if (this.OrbwalkSleeper.IsSleeping)
            {
                return false;
            }

            if (this.CanMove())
            {
                this.OrbwalkSleeper.Sleep(0.2f);
                return this.Owner.BaseUnit.MoveToDirection(GameManager.MousePosition);
            }

            return false;
        }

        public void Dispose()
        {
            this.powershot.Dispose();
        }

        public override bool Orbwalk(Unit9 target, bool attack, bool move, ComboModeMenu comboMenu = null)
        {
            var focusFireTarget = this.windranger?.FocusFireActive == true && this.windranger.FocusFireTarget?.Equals(target) == true;
            if (focusFireTarget && target.IsReflectingDamage)
            {
                return this.DirectionalMove();
            }

            if (focusFireTarget && this.Owner.HasModifier("modifier_windrunner_windrun_invis"))
            {
                focusFireTarget = false;
            }

            return base.Orbwalk(target, !focusFireTarget, move, comboMenu);
        }

        protected override bool MoveComboUseBuffs(AbilityHelper abilityHelper)
        {
            if (base.MoveComboUseBuffs(abilityHelper))
            {
                return true;
            }

            if (abilityHelper.UseMoveAbility(this.windrunMove))
            {
                return true;
            }

            return false;
        }

        protected override bool MoveComboUseDisables(AbilityHelper abilityHelper)
        {
            if (base.MoveComboUseDisables(abilityHelper))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.shackleshot))
            {
                return true;
            }

            return false;
        }
    }
}
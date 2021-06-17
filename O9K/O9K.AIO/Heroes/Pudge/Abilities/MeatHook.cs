namespace O9K.AIO.Heroes.Pudge.Abilities
{
    using System;
    using System.Linq;

    using AIO.Abilities;
    using AIO.Abilities.Menus;
    using AIO.Modes.Combo;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Prediction.Data;

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

    using TargetManager;

    internal class MeatHook : NukeAbility
    {
        private Unit9 lastTarget;

        private float rotation;

        private float rotationTime;

        public MeatHook(ActiveAbility ability)
            : base(ability)
        {
            this.breakShields = this.breakShields.Concat(new[] { "modifier_templar_assassin_refraction_absorb" }).ToArray();
        }

        public override bool CanHit(TargetManager targetManager, IComboModeMenu comboMenu)
        {
            var target = targetManager.Target;
            var predictionInput = this.Ability.GetPredictionInput(target);
            var output = this.Ability.GetPredictionOutput(predictionInput);

            if (output.HitChance <= HitChance.Impossible)
            {
                return false;
            }

            if (target.Distance(this.Owner) < 300 || target.GetImmobilityDuration() > 0)
            {
                return true;
            }

            if (target.Equals(this.lastTarget))
            {
                if (Math.Abs(this.rotation - target.BaseUnit.NetworkRotationRad) > 0.1)
                {
                    this.rotationTime = GameManager.RawGameTime;
                    this.rotation = target.BaseUnit.NetworkRotationRad;
                    return false;
                }

                var menu = comboMenu.GetAbilitySettingsMenu<MeatHookMenu>(this);
                if (this.rotationTime + (menu.Delay / 1000f) > GameManager.RawGameTime)
                {
                    return false;
                }
            }
            else
            {
                this.lastTarget = target;
                this.rotationTime = GameManager.RawGameTime;
                this.rotation = target.BaseUnit.NetworkRotationRad;
                return false;
            }

            return true;
        }

        public override UsableAbilityMenu GetAbilityMenu(string simplifiedName)
        {
            return new MeatHookMenu(this.Ability, simplifiedName);
        }

        public override bool ShouldCast(TargetManager targetManager)
        {
            var target = targetManager.Target;

            if (target.HasModifier("modifier_templar_assassin_refraction_absorb") && target.Distance(this.Owner) < 300)
            {
                return false;
            }

            return base.ShouldCast(targetManager);
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            if (!base.UseAbility(targetManager, comboSleeper, aoe))
            {
                return false;
            }

            if (this.lastTarget != null)
            {
                this.lastTarget.RefreshUnitState();
                this.lastTarget = null;
            }

            var delay = this.Ability.GetHitTime(targetManager.Target);
            this.OrbwalkSleeper.Sleep(delay + (this.Owner.Distance(targetManager.Target) / this.Ability.Speed));
            comboSleeper.Sleep(delay + 0.1f);

            return true;
        }
    }
}
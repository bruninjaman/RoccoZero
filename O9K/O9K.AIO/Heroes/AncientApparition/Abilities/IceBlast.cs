namespace O9K.AIO.Heroes.AncientApparition.Abilities
{
    using System.Linq;

    using AIO.Abilities;
    using AIO.Abilities.Menus;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;
    using Core.Extensions;
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
    using Divine.Extensions;

    using Modes.Combo;

    using Divine.Numerics;

    using TargetManager;

    internal class IceBlast : AoeAbility
    {
        private readonly Core.Entities.Abilities.Heroes.AncientApparition.IceBlast iceBlast;

        private Vector3 direction;

        public IceBlast(ActiveAbility ability)
            : base(ability)
        {
            this.iceBlast = (Core.Entities.Abilities.Heroes.AncientApparition.IceBlast)ability;
        }

        public override bool CanHit(TargetManager targetManager, IComboModeMenu comboMenu)
        {
            var menu = comboMenu.GetAbilitySettingsMenu<IceBlastMenu>(this);
            if (menu.StunOnly && targetManager.Target.GetImmobilityDuration() < 1.5f)
            {
                return false;
            }

            return base.CanHit(targetManager, comboMenu);
        }

        public override UsableAbilityMenu GetAbilityMenu(string simplifiedName)
        {
            return new IceBlastMenu(this.Ability, simplifiedName);
        }

        public bool Release(TargetManager targetManager, Sleeper comboSleeper)
        {
            if (this.iceBlast.IsUsable)
            {
                return false;
            }

            var iceBlastUnit = EntityManager.GetEntities<Unit>()
                .FirstOrDefault(
                    x => x.IsValid && x.NetworkName == "CDOTA_BaseNPC" && x.DayVision == 550 && x.Health == 150
                         && x.Team == this.Owner.Team);

            if (iceBlastUnit == null)
            {
                return true;
            }

            var currentPosition = iceBlastUnit.Position;
            var nextPosition = iceBlastUnit.Position.Extend2D(this.direction, 50);
            var targetPosition = targetManager.Target.GetPredictedPosition(this.iceBlast.GetReleaseFlyTime(iceBlastUnit.Position));

            if (currentPosition.Distance2D(targetPosition) > nextPosition.Distance2D(targetPosition))
            {
                return false;
            }

            if (!this.Ability.UseAbility())
            {
                return false;
            }

            this.Sleeper.Sleep(0.3f);
            this.OrbwalkSleeper.Sleep(0.1f);
            comboSleeper.Sleep(0.1f);
            return true;
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            if (!this.iceBlast.IsUsable)
            {
                return false;
            }

            var input = this.Ability.GetPredictionInput(targetManager.Target, targetManager.EnemyHeroes);
            input.Delay += this.iceBlast.GetReleaseFlyTime(targetManager.Target.Position);
            var output = this.Ability.GetPredictionOutput(input);

            if (output.HitChance < HitChance.Low)
            {
                return false;
            }

            if (!this.Ability.UseAbility(output.CastPosition))
            {
                return false;
            }

            this.direction = this.Owner.Position.Extend2D(output.CastPosition, 9999);

            var delay = this.Ability.GetCastDelay(targetManager.Target);
            comboSleeper.Sleep(delay);
            this.Sleeper.Sleep(delay + 0.1f);
            this.OrbwalkSleeper.Sleep(delay);
            return true;
        }

        protected override bool ChainStun(Unit9 target, bool invulnerability)
        {
            var immobile = invulnerability ? target.GetInvulnerabilityDuration() : target.GetImmobilityDuration();
            if (immobile <= 0)
            {
                return false;
            }

            var hitTime = this.Ability.GetHitTime(target) + this.iceBlast.GetReleaseFlyTime(target.Position);
            if (target.IsInvulnerable)
            {
                hitTime -= 0.1f;
            }

            return hitTime > immobile;
        }
    }
}
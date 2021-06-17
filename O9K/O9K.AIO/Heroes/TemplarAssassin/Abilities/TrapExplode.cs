namespace O9K.AIO.Heroes.TemplarAssassin.Abilities
{
    using System.Linq;

    using AIO.Abilities;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Heroes.TemplarAssassin;
    using Core.Helpers;
    using Core.Managers.Entity;

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

    internal class TrapExplode : DebuffAbility
    {
        private readonly SelfTrap trap;

        public TrapExplode(ActiveAbility ability)
            : base(ability)
        {
            this.trap = (SelfTrap)ability;
        }

        public override bool ShouldCast(TargetManager targetManager)
        {
            var target = targetManager.Target;
            var modifier = target.GetModifier(this.Debuff.DebuffModifierName);

            if (modifier?.RemainingTime > 2.5f)
            {
                return false;
            }

            if (target.IsDarkPactProtected)
            {
                return false;
            }

            if (target.IsInvulnerable || target.IsRooted || target.IsStunned || target.IsHexed)
            {
                return false;
            }

            if (targetManager.TargetSleeper.IsSleeping)
            {
                return false;
            }

            if (this.trap.IsFullyCharged)
            {
                return true;
            }

            if (this.Owner.Distance(target) > this.Ability.Radius * 0.6f && target.GetAngle(this.Owner.Position) > 2)
            {
                return true;
            }

            return false;
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            var templar = this.Owner.Owner;
            var closestTrap = EntityManager9.Units
                .Where(x => x.Name == "npc_dota_templar_assassin_psionic_trap" && x.IsControllable && x.IsAlive)
                .OrderBy(x => x.Distance(templar))
                .FirstOrDefault();

            if (closestTrap != null && closestTrap == this.Owner)
            {
                var explodeAbility = templar.Abilities.FirstOrDefault(x => x.Id == AbilityId.templar_assassin_trap) as ActiveAbility;
                if (explodeAbility?.CanBeCasted() == true && explodeAbility.UseAbility())
                {
                    comboSleeper.Sleep(0.1f);
                    this.OrbwalkSleeper.Sleep(0.1f);
                    targetManager.TargetSleeper.Sleep(0.3f);
                    return true;
                }
            }

            if (!base.UseAbility(targetManager, comboSleeper, aoe))
            {
                return false;
            }

            targetManager.TargetSleeper.Sleep(0.3f);
            return true;
        }
    }
}
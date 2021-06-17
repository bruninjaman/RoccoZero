namespace O9K.AIO.Heroes.Dynamic.Abilities.Nukes.Unique
{
    using AIO.Modes.Combo;

    using Core.Entities.Abilities.Base.Types;
    using Core.Entities.Abilities.Heroes.DarkWillow;
    using Core.Entities.Metadata;
    using Core.Entities.Units;

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

    [AbilityId(AbilityId.dark_willow_shadow_realm)]
    internal class ShadowRealmNukeAbility : OldNukeAbility
    {
        private readonly ShadowRealm shadowRealm;

        public ShadowRealmNukeAbility(INuke ability)
            : base(ability)
        {
            this.shadowRealm = (ShadowRealm)ability;
        }

        public override bool CanBeCasted(ComboModeMenu menu)
        {
            if (menu.IsAbilityEnabled(this.Ability))
            {
                return false;
            }

            if (this.AbilitySleeper.IsSleeping(this.Ability.Handle) || this.OrbwalkSleeper.IsSleeping(this.Ability.Owner.Handle))
            {
                return false;
            }

            if (this.shadowRealm.Casted)
            {
                return this.Ability.Owner.CanAttack();
            }

            return this.Ability.CanBeCasted();
        }

        public override bool ShouldCast(Unit9 target)
        {
            if (this.Ability.UnitTargetCast && !target.IsVisible)
            {
                return false;
            }

            if (!this.shadowRealm.Casted)
            {
                return true;
            }

            if (target.IsReflectingDamage)
            {
                return false;
            }

            if (target.IsInvulnerable)
            {
                return false;
            }

            var damage = this.Nuke.GetDamage(target);

            if (damage <= 0)
            {
                return false;
            }

            if (!this.shadowRealm.DamageMaxed && damage < target.Health)
            {
                return false;
            }

            return true;
        }

        public override bool Use(Unit9 target)
        {
            if (this.shadowRealm.Casted)
            {
                if (!this.Ability.Owner.BaseUnit.Attack(target.BaseUnit))
                {
                    return false;
                }

                this.AbilitySleeper.Sleep(this.Ability.Handle, this.Ability.GetHitTime(target) + 0.5f);
                this.OrbwalkSleeper.Sleep(this.Ability.Owner.Handle, this.Ability.Owner.GetAttackPoint());

                return true;
            }

            if (!this.Ability.UseAbility())
            {
                return false;
            }

            this.OrbwalkSleeper.Sleep(this.Ability.Owner.Handle, this.Ability.GetCastDelay(this.Ability.Owner));
            this.AbilitySleeper.Sleep(this.Ability.Handle, this.Ability.GetHitTime(this.Ability.Owner) + 0.5f);

            return true;
        }
    }
}
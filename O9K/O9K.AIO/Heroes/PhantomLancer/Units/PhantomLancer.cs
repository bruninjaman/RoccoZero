namespace O9K.AIO.Heroes.PhantomLancer.Units
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using AIO.Abilities;
    using AIO.Abilities.Items;

    using Base;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;
    using Core.Entities.Units;
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

    using Modes.Combo;

    using TargetManager;

    [UnitName(nameof(HeroId.npc_dota_hero_phantom_lancer))]
    internal class PhantomLancer : ControllableUnit
    {
        private readonly Sleeper moveSleeper = new Sleeper();

        private DisableAbility abyssal;

        private DisableAbility bloodthorn;

        private DebuffAbility diffusal;

        private Doppelganger doppel;

        private BlinkAbility doppelBlink;

        private NukeAbility lance;

        private BuffAbility manta;

        private Nullifier nullifier;

        private DisableAbility orchid;

        public PhantomLancer(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.phantom_lancer_spirit_lance, x => this.lance = new NukeAbility(x) },
                { AbilityId.phantom_lancer_doppelwalk, x => this.doppel = new Doppelganger(x) },

                { AbilityId.item_diffusal_blade, x => this.diffusal = new DebuffAbility(x) },
                { AbilityId.item_abyssal_blade, x => this.abyssal = new DisableAbility(x) },
                { AbilityId.item_manta, x => this.manta = new BuffAbility(x) },
                { AbilityId.item_bloodthorn, x => this.bloodthorn = new Bloodthorn(x) },
                { AbilityId.item_orchid, x => this.orchid = new DisableAbility(x) },
                { AbilityId.item_nullifier, x => this.nullifier = new Nullifier(x) },
            };

            this.MoveComboAbilities.Add(AbilityId.phantom_lancer_doppelwalk, x => this.doppelBlink = new BlinkAbility(x));
        }

        protected override int BodyBlockRange { get; } = 80;

        public override bool CanAttack(Unit9 target, float additionalRange = 0)
        {
            var rush = (ToggleAbility)this.Owner.Abilities.FirstOrDefault(x => x.Id == AbilityId.phantom_lancer_phantom_edge);
            if (rush?.Enabled == false && rush.CanBeCasted())
            {
                additionalRange = rush.Range - 100;
            }

            var canAttack = base.CanAttack(target, additionalRange);

            if (canAttack && additionalRange > 0)
            {
                this.ComboSleeper.Sleep(0.3f);
            }

            return canAttack;
        }

        public override bool CanMove()
        {
            if (!base.CanMove())
            {
                return false;
            }

            if (this.Owner.HasModifier("modifier_phantom_lancer_phantom_edge_boost"))
            {
                return false;
            }

            return true;
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            if (this.Owner.HasModifier("modifier_phantom_lancer_phantom_edge_boost"))
            {
                return false;
            }

            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.UseAbility(this.abyssal))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.nullifier))
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

            if (abilityHelper.UseAbility(this.diffusal))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.lance))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.doppel))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.manta, this.Owner.GetAttackRange()))
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
            if (this.moveSleeper || this.Owner.HasModifier("modifier_phantom_lancer_phantom_edge_boost"))
            {
                return false;
            }

            if (base.MoveComboUseBlinks(abilityHelper))
            {
                return true;
            }

            var rush = (ToggleAbility)this.Owner.Abilities.FirstOrDefault(x => x.Id == AbilityId.phantom_lancer_phantom_edge);

            if (rush?.Enabled == false && rush.CanBeCasted())
            {
                var mousePosition = GameManager.MousePosition;
                var range = rush.Range;
                var target = EntityManager9.Units
                    .Where(
                        x => x.IsUnit && x.IsVisible && x.IsAlive && !x.IsAlly(this.Owner) && !x.IsInvulnerable && !x.IsAttackImmune
                             && x.Distance(this.Owner) < range)
                    .OrderBy(x => x.DistanceSquared(mousePosition))
                    .FirstOrDefault(x => x.Distance(mousePosition) < 700 && x.Distance(this.Owner) > 500);

                if (target != null && target.Distance(mousePosition) < this.Owner.Distance(mousePosition))
                {
                    if (this.Owner.Attack(target))
                    {
                        this.moveSleeper.Sleep(0.3f);
                        this.OrbwalkSleeper.Sleep(0.3f);
                        return true;
                    }

                    return false;
                }
            }

            if (abilityHelper.UseMoveAbility(this.doppelBlink))
            {
                return true;
            }

            if (this.Owner.IsAttacking)
            {
                this.OrbwalkSleeper.Reset();
                this.MoveSleeper.Reset();
                return true;
            }

            //if (this.moveIllusion?.IsValid != true)
            //{
            //    this.moveIllusion = EntityManager9.Units.Where(x => x.IsIllusion && x.IsAlly(Owner) && x.IsMyControllable && x.IsAlive)
            //        .OrderBy(x => x.DistanceSquared(Owner.Position))
            //        .FirstOrDefault();

            //    if (this.moveIllusion != null)
            //    {
            //        this.moveIllusion.Move(Vector3.Zero);
            //        return true;
            //    }
            //}

            return false;
        }

        //private Unit9 moveIllusion;
    }
}
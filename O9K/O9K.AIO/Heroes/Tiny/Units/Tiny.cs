namespace O9K.AIO.Heroes.Tiny.Units
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

    [UnitName(nameof(HeroId.npc_dota_hero_tiny))]
    internal class Tiny : ControllableUnit
    {
        private DisableAbility avalanche;

        private BlinkAbility blink;

        private DisableAbility bloodthorn;

        private NukeAbility dagon;

        private EtherealBlade ethereal;

        private DisableAbility orchid;

        private SpeedBuffAbility phase;

        private NukeAbility toss;

        private TreeGrab treeGrab;

        private TreeThrow treeThrow;

        private DebuffAbility veil;

        private TreeVolley volley;

        public Tiny(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.tiny_avalanche, x => this.avalanche = new DisableAbility(x) },
                { AbilityId.tiny_toss, x => this.toss = new Toss(x) },
                { AbilityId.tiny_tree_grab, x => this.treeGrab = new TreeGrab(x) },
                { AbilityId.tiny_toss_tree, x => this.treeThrow = new TreeThrow(x) },
                { AbilityId.tiny_tree_channel, x => this.volley = new TreeVolley(x) },

                { AbilityId.item_phase_boots, x => this.phase = new SpeedBuffAbility(x) },
                { AbilityId.item_ethereal_blade, x => this.ethereal = new EtherealBlade(x) },
                { AbilityId.item_dagon_5, x => this.dagon = new NukeAbility(x) },
                { AbilityId.item_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_orchid, x => this.orchid = new DisableAbility(x) },
                { AbilityId.item_veil_of_discord, x => this.veil = new DebuffAbility(x) },
                { AbilityId.item_bloodthorn, x => this.bloodthorn = new Bloodthorn(x) },
            };

            this.MoveComboAbilities.Add(AbilityId.leshrac_split_earth, _ => this.avalanche);
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.UseAbility(this.blink, 400, 0))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.veil))
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

            if (abilityHelper.UseAbility(this.avalanche))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.ethereal))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.toss))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.dagon))
            {
                return true;
            }

            if (!abilityHelper.CanBeCasted(this.avalanche, false) && abilityHelper.UseAbility(this.volley))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.treeGrab))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.treeThrow))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.phase))
            {
                return true;
            }

            return false;
        }

        public void Toss()
        {
            var tossAbility = this.Owner.Abilities.FirstOrDefault(x => x.Id == AbilityId.tiny_toss) as ActiveAbility;
            if (tossAbility?.CanBeCasted() != true)
            {
                return;
            }

            var tower = EntityManager9.Units.Where(x => x.IsTower && x.IsAlly(this.Owner) && x.IsAlive)
                .OrderBy(x => x.Distance(this.Owner))
                .FirstOrDefault(x => x.Distance(this.Owner) < 2000);
            if (tower == null)
            {
                return;
            }

            var tossTarget = EntityManager9.Units
                .Where(
                    x => x.IsUnit && !x.IsInvulnerable && !x.IsMagicImmune && x.IsAlive && x.IsVisible
                         && x.Distance(this.Owner) < tossAbility.CastRange && x.Distance(tower) < tower.GetAttackRange())
                .OrderBy(x => x.Distance(tower))
                .FirstOrDefault();
            if (tossTarget == null)
            {
                return;
            }

            var grabUnit = EntityManager9.Units
                .Where(
                    x => x.IsUnit && !x.Equals(this.Owner) && !x.IsInvulnerable && !x.IsMagicImmune && x.IsAlive && x.IsVisible
                         && x.Distance(this.Owner) < tossAbility.Radius)
                .OrderBy(x => x.Distance(this.Owner))
                .FirstOrDefault();

            if (grabUnit?.IsHero != true || grabUnit.IsIllusion || grabUnit.IsAlly(this.Owner))
            {
                return;
            }

            tossAbility.UseAbility(tossTarget);
        }

        protected override bool MoveComboUseDisables(AbilityHelper abilityHelper)
        {
            if (base.MoveComboUseDisables(abilityHelper))
            {
                return true;
            }

            if (abilityHelper.UseMoveAbility(this.avalanche))
            {
                return true;
            }

            return false;
        }
    }
}
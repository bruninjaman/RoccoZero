namespace O9K.AIO.Heroes.Dynamic.Units
{
    using System.Collections.Generic;

    using Abilities;
    using Abilities.Blinks;
    using Abilities.Buffs;
    using Abilities.Debuffs;
    using Abilities.Disables;
    using Abilities.Harasses;
    using Abilities.Nukes;
    using Abilities.Shields;
    using Abilities.Specials;

    using AIO.Modes.Combo;
    using AIO.Modes.MoveCombo;

    using Base;

    using Core.Entities.Abilities.Base;
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

    using TargetManager;

    internal class DynamicUnit : ControllableUnit
    {
        private readonly List<IOldAbilityGroup> groups = new List<IOldAbilityGroup>();

        private readonly HashSet<AbilityId> ignoredAbilities = new HashSet<AbilityId>
        {
            AbilityId.ember_spirit_activate_fire_remnant,
            AbilityId.item_tpscroll,
            AbilityId.item_recipe_travel_boots,
            AbilityId.item_recipe_travel_boots_2,
            AbilityId.item_enchanted_mango,
            AbilityId.item_ghost,
            AbilityId.item_hand_of_midas,
            AbilityId.item_dust,
            AbilityId.item_mekansm,
            AbilityId.item_guardian_greaves,
            AbilityId.item_glimmer_cape,
            AbilityId.item_sphere,
            AbilityId.item_bfury,
            AbilityId.item_quelling_blade,
            AbilityId.item_shadow_amulet,
            AbilityId.item_magic_stick,
            AbilityId.item_magic_wand,
            AbilityId.item_bloodstone,
            AbilityId.item_branches,
        };

        public DynamicUnit(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu, BaseHero baseHero)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.Blinks = new BlinkAbilityGroup(baseHero);
            this.Nukes = new NukeAbilityGroup(baseHero);
            this.Debuffs = new DebuffAbilityGroup(baseHero);
            this.Disables = new DisableAbilityGroup(baseHero);
            this.Buffs = new BuffAbilityGroup(baseHero);
            this.Harasses = new HarassAbilityGroup(baseHero);
            this.Shields = new ShieldAbilityGroup(baseHero);
            this.Specials = new SpecialAbilityGroup(baseHero);

            this.Blinks.Disables = this.Disables;
            this.Blinks.Specials = this.Specials;
            this.Disables.Specials = this.Specials;
            this.Buffs.Blinks = this.Blinks;
            this.Disables.Blinks = this.Blinks;
            this.Shields.Blinks = this.Blinks;

            this.groups.Add(this.Nukes);
            this.groups.Add(this.Debuffs);
            this.groups.Add(this.Blinks);
            this.groups.Add(this.Disables);
            this.groups.Add(this.Buffs);
            this.groups.Add(this.Harasses);
            this.groups.Add(this.Shields);
            this.groups.Add(this.Specials);
        }

        public BlinkAbilityGroup Blinks { get; }

        public BuffAbilityGroup Buffs { get; }

        public DebuffAbilityGroup Debuffs { get; }

        public DisableAbilityGroup Disables { get; }

        public HarassAbilityGroup Harasses { get; }

        public NukeAbilityGroup Nukes { get; }

        public ShieldAbilityGroup Shields { get; }

        public SpecialAbilityGroup Specials { get; }

        public override void AddAbility(ActiveAbility ability, IEnumerable<ComboModeMenu> comboMenus, MoveComboModeMenu moveMenu)
        {
            if (this.ignoredAbilities.Contains(ability.Id))
            {
                return;
            }

            foreach (var group in this.groups)
            {
                if (group.AddAbility(ability))
                {
                    foreach (var comboModeMenu in comboMenus)
                    {
                        comboModeMenu.AddComboAbility(ability);
                    }
                }
            }

            this.AddMoveComboAbility(ability, moveMenu);
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var target = targetManager.Target;

            if (this.Buffs.Use(target, comboModeMenu))
            {
                return true;
            }

            if (this.Shields.Use(target, comboModeMenu))
            {
                return true;
            }

            if (this.Debuffs.UseAmplifiers(target, comboModeMenu))
            {
                return true;
            }

            if (this.Disables.UseBlinkDisable(target, comboModeMenu))
            {
                return true;
            }

            if (this.Disables.Use(target, comboModeMenu))
            {
                return true;
            }

            if (this.Debuffs.Use(target, comboModeMenu))
            {
                return true;
            }

            if (this.Nukes.Use(target, comboModeMenu))
            {
                return true;
            }

            if (this.Harasses.Use(target, comboModeMenu))
            {
                return true;
            }

            if (this.Specials.Use(target, comboModeMenu))
            {
                return true;
            }

            if (this.Blinks.Use(target, comboModeMenu))
            {
                return true;
            }

            return false;
        }

        public override void RemoveAbility(ActiveAbility ability)
        {
            foreach (var group in this.groups)
            {
                group.RemoveAbility(ability);
            }
        }
    }
}
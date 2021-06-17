namespace O9K.AIO.Heroes.CrystalMaiden.Units
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

    [UnitName(nameof(HeroId.npc_dota_hero_crystal_maiden))]
    internal class CrystalMaiden : ControllableUnit
    {
        private DisableAbility atos;

        private ShieldAbility bkb;

        private BlinkAbility blink;

        private NukeAbility field;

        private ForceStaff force;

        private DisableAbility frostbite;

        private ShieldAbility glimmer;

        private NukeAbility nova;

        private DisableAbility gungir;

        public CrystalMaiden(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.crystal_maiden_crystal_nova, x => this.nova = new NukeAbility(x) },
                { AbilityId.crystal_maiden_frostbite, x => this.frostbite = new DisableAbility(x) },
                { AbilityId.crystal_maiden_freezing_field, x => this.field = new FreezingField(x) },

                { AbilityId.item_force_staff, x => this.force = new ForceStaff(x) },
                { AbilityId.item_blink, x => this.blink = new BlinkDaggerCM(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkDaggerCM(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkDaggerCM(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkDaggerCM(x) },
                { AbilityId.item_glimmer_cape, x => this.glimmer = new ShieldAbility(x) },
                { AbilityId.item_black_king_bar, x => this.bkb = new ShieldAbility(x) },
                { AbilityId.item_rod_of_atos, x => this.atos = new DisableAbility(x) },
                { AbilityId.item_gungir, x => this.gungir = new DisableAbility(x) },
            };

            this.MoveComboAbilities.Add(AbilityId.crystal_maiden_frostbite, _ => this.frostbite);
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.CanBeCasted(this.field))
            {
                if (abilityHelper.CanBeCasted(this.bkb, false, false))
                {
                    if (abilityHelper.ForceUseAbility(this.bkb))
                    {
                        return true;
                    }
                }

                if (abilityHelper.HasMana(this.field, this.glimmer) && abilityHelper.CanBeCasted(this.glimmer, false, false))
                {
                    if (abilityHelper.ForceUseAbility(this.glimmer))
                    {
                        return true;
                    }
                }

                if (abilityHelper.HasMana(this.field, this.atos) && abilityHelper.UseAbility(this.atos))
                {
                    return true;
                }
                
                if (abilityHelper.HasMana(this.field, this.gungir) && abilityHelper.UseAbility(this.gungir))
                {
                    return true;
                }

                if (abilityHelper.HasMana(this.field, this.frostbite) && abilityHelper.UseAbility(this.frostbite))
                {
                    return true;
                }

                if (abilityHelper.UseAbility(this.field))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbilityIfCondition(this.blink, this.field))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.force, 600, 400))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.atos))
            {
                return true;
            }
            
            if (abilityHelper.UseAbility(this.gungir))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.frostbite))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.nova))
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

            if (abilityHelper.UseMoveAbility(this.frostbite))
            {
                return true;
            }

            return false;
        }
    }
}
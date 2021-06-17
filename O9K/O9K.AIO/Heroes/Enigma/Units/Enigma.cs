namespace O9K.AIO.Heroes.Enigma.Units
{
    using System;
    using System.Collections.Generic;

    using Abilities;

    using AIO.Abilities;

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

    [UnitName(nameof(HeroId.npc_dota_hero_enigma))]
    internal class Enigma : ControllableUnit
    {
        private ShieldAbility bkb;

        private BlackHole blackHole;

        private BlinkDaggerEnigma blink;

        private ShieldAbility ghost;

        private DisableAbility malefice;

        private AoeAbility pulse;

        private UntargetableAbility refresher;

        private UntargetableAbility refresherShard;

        private DebuffAbility shiva;

        public Enigma(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.enigma_malefice, x => this.malefice = new DisableAbility(x) },
                { AbilityId.enigma_midnight_pulse, x => this.pulse = new AoeAbility(x) },
                { AbilityId.enigma_black_hole, x => this.blackHole = new BlackHole(x) },

                { AbilityId.item_black_king_bar, x => this.bkb = new ShieldAbility(x) },
                { AbilityId.item_ghost, x => this.ghost = new ShieldAbility(x) },
                { AbilityId.item_blink, x => this.blink = new BlinkDaggerEnigma(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkDaggerEnigma(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkDaggerEnigma(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkDaggerEnigma(x) },
                { AbilityId.item_refresher, x => this.refresher = new UntargetableAbility(x) },
                { AbilityId.item_refresher_shard, x => this.refresherShard = new UntargetableAbility(x) },
                { AbilityId.item_shivas_guard, x => this.shiva = new DebuffAbility(x) },
            };
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.CanBeCasted(this.blackHole))
            {
                if (abilityHelper.CanBeCasted(this.bkb, false, false))
                {
                    if (abilityHelper.ForceUseAbility(this.bkb))
                    {
                        return true;
                    }
                }

                if (abilityHelper.HasMana(this.pulse, this.blackHole) && abilityHelper.UseAbility(this.pulse))
                {
                    return true;
                }

                if (abilityHelper.HasMana(this.shiva, this.blackHole) && abilityHelper.UseAbility(this.shiva))
                {
                    return true;
                }

                if (abilityHelper.UseAbility(this.blackHole))
                {
                    return true;
                }
            }

            if (abilityHelper.CanBeCastedIfCondition(this.blink, this.blackHole))
            {
                if (abilityHelper.CanBeCasted(this.bkb, false, false))
                {
                    if (abilityHelper.ForceUseAbility(this.bkb))
                    {
                        return true;
                    }
                }

                if (abilityHelper.CanBeCasted(this.ghost, false, false))
                {
                    if (abilityHelper.ForceUseAbility(this.ghost))
                    {
                        return true;
                    }
                }
            }

            if (abilityHelper.UseAbilityIfCondition(this.blink, this.blackHole))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(this.refresher) || abilityHelper.CanBeCasted(this.refresherShard))
            {
                if (abilityHelper.CanBeCasted(this.blackHole, true, true, true, false) && !this.blackHole.Ability.IsReady)
                {
                    var useRefresher = abilityHelper.CanBeCasted(this.refresherShard) ? this.refresherShard : this.refresher;

                    if (abilityHelper.HasMana(this.blackHole, useRefresher))
                    {
                        if (abilityHelper.UseAbility(useRefresher))
                        {
                            return true;
                        }
                    }
                }
            }

            if (abilityHelper.UseAbility(this.malefice))
            {
                return true;
            }

            return false;
        }
    }
}
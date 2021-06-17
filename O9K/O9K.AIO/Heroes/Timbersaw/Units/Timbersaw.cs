namespace O9K.AIO.Heroes.Timbersaw.Units
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

    [UnitName(nameof(HeroId.npc_dota_hero_shredder))]
    internal class Timbersaw : ControllableUnit
    {
        private readonly List<Chakram> chakrams = new List<Chakram>();

        private ShieldAbility bladeMail;

        private BlinkDaggerTimbersaw blink;

        private DisableAbility hex;

        private DebuffAbility shiva;

        private TimberChain timberChain;

        private TimberChainBlink timberChainBlink;

        private NukeAbility whirlingDeath;

        public Timbersaw(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                {
                    AbilityId.shredder_chakram, x =>
                        {
                            var chakram = new Chakram(x);
                            this.chakrams.Add(chakram);
                            return chakram;
                        }
                },
                {
                    AbilityId.shredder_chakram_2, x =>
                        {
                            var chakram = new Chakram(x);
                            this.chakrams.Add(chakram);
                            return chakram;
                        }
                },
                { AbilityId.shredder_whirling_death, x => this.whirlingDeath = new NukeAbility(x) },
                { AbilityId.shredder_timber_chain, x => this.timberChain = new TimberChain(x) },

                { AbilityId.item_blink, x => this.blink = new BlinkDaggerTimbersaw(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkDaggerTimbersaw(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkDaggerTimbersaw(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkDaggerTimbersaw(x) },
                { AbilityId.item_shivas_guard, x => this.shiva = new DebuffAbility(x) },
                { AbilityId.item_sheepstick, x => this.hex = new DisableAbility(x) },
                { AbilityId.item_blade_mail, x => this.bladeMail = new ShieldAbility(x) },
            };

            this.MoveComboAbilities.Add(AbilityId.shredder_timber_chain, x => this.timberChainBlink = new TimberChainBlink(x));
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var damagingChakrams = this.chakrams.Count(x => comboModeMenu.IsAbilityEnabled(x.Ability) && x.IsDamaging(targetManager));
            var returnChakram = this.chakrams.Find(
                x => comboModeMenu.IsAbilityEnabled(x.Ability) && x.ShouldReturnChakram(targetManager, damagingChakrams));
            if (returnChakram?.Return() == true)
            {
                return true;
            }

            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.UseAbility(this.whirlingDeath))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.hex))
            {
                return true;
            }

            if (abilityHelper.UseAbilityIfCondition(this.blink, this.timberChain, this.whirlingDeath))
            {
                if (abilityHelper.CanBeCasted(this.whirlingDeath, false, false))
                {
                    abilityHelper.ForceUseAbility(this.whirlingDeath, true);
                }

                return true;
            }

            if (abilityHelper.UseAbility(this.shiva))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.bladeMail, 400))
            {
                return true;
            }

            if (abilityHelper.UseAbilityIfCondition(this.timberChain, this.blink))
            {
                return true;
            }

            var chakram = this.chakrams.Find(x => x.Ability.CanBeCasted());

            if (abilityHelper.UseAbility(chakram))
            {
                return true;
            }

            return false;
        }

        public override void EndCombo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            base.EndCombo(targetManager, comboModeMenu);

            foreach (var chakram in this.chakrams.Where(x => x.ReturnChakram.CanBeCasted() && comboModeMenu.IsAbilityEnabled(x.Ability)))
            {
                chakram.Return();
            }
        }

        protected override bool MoveComboUseBlinks(AbilityHelper abilityHelper)
        {
            if (base.MoveComboUseBlinks(abilityHelper))
            {
                return true;
            }

            if (abilityHelper.UseMoveAbility(this.timberChainBlink))
            {
                return true;
            }

            return false;
        }
    }
}
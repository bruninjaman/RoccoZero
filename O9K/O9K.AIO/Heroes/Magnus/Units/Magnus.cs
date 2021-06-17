namespace O9K.AIO.Heroes.Magnus.Units
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
    using Core.Extensions;
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

    using Modes;

    using TargetManager;

    [UnitName(nameof(HeroId.npc_dota_hero_magnataur))]
    internal class Magnus : ControllableUnit
    {
        private BlinkDaggerMagnus blink;

        private ForceStaff force;

        private BlinkAbility moveSkewer;

        private ReversePolarity polarity;

        private UntargetableAbility refresher;

        private UntargetableAbility refresherShard;

        private ShivasGuard shiva;

        private Shockwave shockwave;

        private Skewer skewer;

        public Magnus(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.magnataur_shockwave, x => this.shockwave = new Shockwave(x) },
                {
                    AbilityId.magnataur_skewer, x =>
                        {
                            this.skewer = new Skewer(x);
                            this.polarity?.AddSkewer(this.skewer);
                            return this.skewer;
                        }
                },
                {
                    AbilityId.magnataur_reverse_polarity, x =>
                        {
                            this.polarity = new ReversePolarity(x);
                            if (this.skewer != null)
                            {
                                this.polarity.AddSkewer(this.skewer);
                            }

                            return this.polarity;
                        }
                },

                { AbilityId.item_blink, x => this.blink = new BlinkDaggerMagnus(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkDaggerMagnus(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkDaggerMagnus(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkDaggerMagnus(x) },
                { AbilityId.item_force_staff, x => this.force = new ForceStaff(x) },
                { AbilityId.item_shivas_guard, x => this.shiva = new ShivasGuard(x) },
                { AbilityId.item_refresher, x => this.refresher = new UntargetableAbility(x) },
                { AbilityId.item_refresher_shard, x => this.refresherShard = new UntargetableAbility(x) },
            };

            this.MoveComboAbilities.Add(AbilityId.magnataur_skewer, x => this.moveSkewer = new BlinkAbility(x));
        }

        public void BlinkSkewerCombo(TargetManager targetManager, BlinkSkewerModeMenu menu)
        {
            if (this.ComboSleeper.IsSleeping)
            {
                return;
            }

            if (this.blink?.Ability.CanBeCasted() != true || this.skewer?.Ability.CanBeCasted() != true)
            {
                return;
            }

            var target = targetManager.Target;
            var castPosition = target.GetPredictedPosition(this.skewer.Ability.CastPoint);
            var blinkPosition = this.Owner.Position.Extend2D(castPosition, this.Owner.Distance(castPosition) + 100);
            var distance = this.Owner.Distance(blinkPosition);

            if (this.blink.Ability.CastRange < distance)
            {
                return;
            }

            var ally = targetManager.AllyHeroes
                .Where(
                    x => !x.Equals(this.Owner) && menu.IsAllyEnabled(x.Name)
                                               && x.Distance(blinkPosition) < this.skewer.Ability.CastRange + 800)
                .OrderBy(x => x.Distance(blinkPosition))
                .FirstOrDefault();

            if (ally == null)
            {
                return;
            }

            this.OrbwalkSleeper.Sleep(this.skewer.Ability.CastPoint + 0.3f);

            this.blink.Ability.UseAbility(blinkPosition);
            this.skewer.Ability.UseAbility(ally.Position);
            this.ComboSleeper.Sleep(0.3f);
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            if (abilityHelper.UseAbility(this.polarity))
            {
                return true;
            }

            if (!abilityHelper.CanBeCasted(this.polarity, false, false, false) && !abilityHelper.CanBeCasted(this.skewer))
            {
                if (abilityHelper.UseAbility(this.shiva))
                {
                    return true;
                }
            }

            if (abilityHelper.UseDoubleBlinkCombo(this.force, this.blink))
            {
                return true;
            }

            if (abilityHelper.UseAbilityIfCondition(this.blink, this.polarity))
            {
                UpdateManager.BeginInvoke(50, () => this.polarity.ForceUseAbility(targetManager, this.ComboSleeper, comboModeMenu));
                return true;
            }

            if (abilityHelper.UseAbility(this.force, 500, 100))
            {
                return true;
            }

            if (abilityHelper.UseAbility(this.skewer))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(this.skewer, false, false))
            {
                var canCastBlink = abilityHelper.CanBeCasted(this.blink, false, false);
                var canCastForce = abilityHelper.CanBeCasted(this.force, false, false);

                if (this.skewer.UseAbilityIfCondition(
                    targetManager,
                    this.ComboSleeper,
                    comboModeMenu,
                    this.polarity,
                    canCastBlink,
                    canCastForce))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbilityIfCondition(this.skewer, this.blink, this.force))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(this.skewer, false, false) && abilityHelper.CanBeCasted(this.shockwave, false)
                                                                     && !abilityHelper.CanBeCasted(this.polarity, false, false))
            {
                if (this.skewer.UseAbilityOnTarget(targetManager, this.ComboSleeper))
                {
                    return true;
                }
            }

            if (abilityHelper.UseAbilityIfCondition(this.shockwave, this.blink, this.force, this.skewer, this.polarity))
            {
                return true;
            }

            if (abilityHelper.CanBeCasted(this.refresher) || abilityHelper.CanBeCasted(this.refresherShard))
            {
                if (abilityHelper.CanBeCasted(this.polarity, true, true, true, false) && !this.polarity.Ability.IsReady)
                {
                    var useRefresher = abilityHelper.CanBeCasted(this.refresherShard) ? this.refresherShard : this.refresher;

                    if (abilityHelper.HasMana(this.polarity, useRefresher))
                    {
                        if (abilityHelper.UseAbility(useRefresher))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected override bool MoveComboUseBlinks(AbilityHelper abilityHelper)
        {
            if (base.MoveComboUseBlinks(abilityHelper))
            {
                return true;
            }

            if (abilityHelper.UseMoveAbility(this.moveSkewer))
            {
                return true;
            }

            return false;
        }
    }
}
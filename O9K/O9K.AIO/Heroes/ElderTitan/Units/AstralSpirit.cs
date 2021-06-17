namespace O9K.AIO.Heroes.ElderTitan.Units
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

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

    [UnitName("npc_dota_elder_titan_ancestral_spirit")]
    internal class AstralSpirit : ControllableUnit
    {
        private readonly List<Unit9> damagedUnits = new List<Unit9>();

        private readonly List<Unit9> moveUnits = new List<Unit9>();

        private DisableAbility stomp;

        public AstralSpirit(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                { AbilityId.elder_titan_echo_stomp_spirit, x => this.stomp = new DisableAbility(x) },
            };
        }

        public override bool CanMove()
        {
            return !this.MoveSleeper.IsSleeping;
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

            foreach (var unit in targetManager.EnemyUnits)
            {
                if (this.Owner.Distance(unit) < 275)
                {
                    if (!this.damagedUnits.Contains(unit))
                    {
                        this.damagedUnits.Add(unit);
                    }

                    if (this.moveUnits.Contains(unit))
                    {
                        this.moveUnits.Remove(unit);
                    }
                }
                else if (!this.damagedUnits.Contains(unit))
                {
                    if (!this.moveUnits.Contains(unit))
                    {
                        this.moveUnits.Add(unit);
                    }
                }
            }

            if (abilityHelper.UseAbility(this.stomp))
            {
                return true;
            }

            return false;
        }

        public override void EndCombo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            base.EndCombo(targetManager, comboModeMenu);
            this.damagedUnits.Clear();
            this.moveUnits.Clear();
        }

        public override bool Orbwalk(Unit9 target, bool attack, bool move, ComboModeMenu comboMenu = null)
        {
            if (target != null)
            {
                if (comboMenu?.IsAbilityEnabled(this.stomp.Ability) == true && this.stomp.Ability.CanBeCasted())
                {
                    return this.Move(target.GetPredictedPosition(1f));
                }

                if (this.damagedUnits.Contains(target))
                {
                    var moveTarget = this.moveUnits.Where(x => x.IsValid && x.IsAlive && x.Distance(this.Owner) < 1000)
                        .OrderByDescending(x => x.IsHero)
                        .ThenBy(x => x.Distance(this.Owner))
                        .FirstOrDefault();

                    if (moveTarget != null)
                    {
                        return this.Move(moveTarget.Position);
                    }

                    var spiritReturn =
                        this.Owner.Abilities.FirstOrDefault(x => x.Id == AbilityId.elder_titan_return_spirit) as ActiveAbility;

                    if (spiritReturn?.CanBeCasted() == true && spiritReturn.UseAbility())
                    {
                        this.OrbwalkSleeper.Sleep(3f);
                        return true;
                    }
                }
                else
                {
                    return this.Move(target.Position);
                }
            }

            return base.Orbwalk(target, attack, move, comboMenu);
        }
    }
}
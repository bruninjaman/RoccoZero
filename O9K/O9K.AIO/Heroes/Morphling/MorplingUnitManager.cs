namespace O9K.AIO.Heroes.Morphling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Heroes.Unique;
    using Core.Logger;

    using Dynamic.Units;

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

    using Divine.Numerics;

    using UnitManager;

    internal class MorphlingUnitManager : UnitManager
    {
        public MorphlingUnitManager(BaseHero baseHero)
            : base(baseHero)
        {
        }

        public override void ExecuteCombo(ComboModeMenu comboModeMenu)
        {
            var controlMorphedUnitName = this.owner.Hero is Morphling morphling && morphling.IsMorphed
                                             ? morphling.MorphedHero.Name
                                             : this.owner.HeroName;

            foreach (var controllable in this.ControllableUnits.Where(
                x => x.MorphedUnitName == null || x.MorphedUnitName == controlMorphedUnitName))
            {
                if (controllable.ComboSleeper.IsSleeping)
                {
                    continue;
                }

                if (!comboModeMenu.IgnoreInvisibility && controllable.IsInvisible)
                {
                    return;
                }

                if (controllable.Combo(this.targetManager, comboModeMenu))
                {
                    controllable.LastMovePosition = Vector3.Zero;
                }
            }
        }

        public override void Orbwalk(ComboModeMenu comboModeMenu)
        {
            if (this.issuedAction.IsSleeping)
            {
                return;
            }

            var controlMorphedUnitName = this.owner.Hero is Morphling morphling && morphling.IsMorphed
                                             ? morphling.MorphedHero.Name
                                             : this.owner.HeroName;

            var allUnits = this.ControllableUnits.Where(x => x.MorphedUnitName == null || x.MorphedUnitName == controlMorphedUnitName)
                .OrderBy(x => this.IssuedActionTime(x.Handle))
                .ToList();

            if (this.BodyBlock(allUnits, comboModeMenu))
            {
                this.issuedAction.Sleep(0.05f);
                return;
            }

            var noOrbwalkUnits = new List<ControllableUnit>();
            foreach (var controllable in allUnits)
            {
                if (!controllable.OrbwalkEnabled)
                {
                    noOrbwalkUnits.Add(controllable);
                    continue;
                }

                if (this.unitIssuedAction.IsSleeping(controllable.Handle))
                {
                    continue;
                }

                if (!controllable.Orbwalk(this.targetManager.Target, comboModeMenu))
                {
                    continue;
                }

                this.issuedActionTimings[controllable.Handle] = GameManager.RawGameTime;
                this.unitIssuedAction.Sleep(controllable.Handle, 0.2f);
                this.issuedAction.Sleep(0.05f);
                return;
            }

            if (noOrbwalkUnits.Count > 0 && !this.unitIssuedAction.IsSleeping(uint.MaxValue))
            {
                this.ControlAllUnits(noOrbwalkUnits);
            }
        }

        protected override void OnAbilityAdded(Ability9 entity)
        {
            try
            {
                if (!entity.IsControllable || entity.IsFake || !entity.Owner.IsAlly(this.owner.Team)
                    || !(entity is ActiveAbility activeAbility))
                {
                    return;
                }

                var abilityOwner = entity.Owner;
                var morph = entity.Owner as Morphling;

                if (morph?.IsMorphed == true)
                {
                    ControllableUnit morphedUnit;

                    if (this.unitTypes.TryGetValue(morph.MorphedHero.Name, out var type))
                    {
                        morphedUnit = this.controllableUnits.Find(x => x.Handle == abilityOwner.Handle && x.GetType() == type);

                        if (morphedUnit == null)
                        {
                            morphedUnit = (ControllableUnit)Activator.CreateInstance(
                                type,
                                abilityOwner,
                                this.abilitySleeper,
                                this.orbwalkSleeper[abilityOwner.Handle],
                                this.GetUnitMenu(abilityOwner));
                            morphedUnit.FailSafe = this.BaseHero.FailSafe;
                            morphedUnit.MorphedUnitName = morph.MorphedHero.Name;

                            foreach (var item in abilityOwner.Abilities.Where(x => x.IsItem).OfType<ActiveAbility>())
                            {
                                morphedUnit.AddAbility(item, this.BaseHero.ComboMenus, this.BaseHero.MoveComboModeMenu);
                            }

                            this.controllableUnits.Add(morphedUnit);
                        }
                    }
                    else
                    {
                        morphedUnit = this.controllableUnits.Find(x => x.Handle == abilityOwner.Handle && x is DynamicUnit);

                        if (morphedUnit == null)
                        {
                            morphedUnit = new DynamicUnit(
                                abilityOwner,
                                this.abilitySleeper,
                                this.orbwalkSleeper[abilityOwner.Handle],
                                this.GetUnitMenu(abilityOwner),
                                this.BaseHero)
                            {
                                FailSafe = this.BaseHero.FailSafe,
                                MorphedUnitName = morph.MorphedHero.Name
                            };
                            foreach (var item in abilityOwner.Abilities.Where(x => x.IsItem).OfType<ActiveAbility>())
                            {
                                morphedUnit.AddAbility(item, this.BaseHero.ComboMenus, this.BaseHero.MoveComboModeMenu);
                            }

                            this.controllableUnits.Add(morphedUnit);
                        }
                    }

                    if (activeAbility.IsItem)
                    {
                        foreach (var controllableUnit in this.controllableUnits.Where(x => x.Handle == abilityOwner.Handle))
                        {
                            controllableUnit.AddAbility(activeAbility, this.BaseHero.ComboMenus, this.BaseHero.MoveComboModeMenu);
                        }
                    }
                    else
                    {
                        morphedUnit.AddAbility(activeAbility, this.BaseHero.ComboMenus, this.BaseHero.MoveComboModeMenu);
                    }

                    return;
                }

                if (activeAbility.IsItem)
                {
                    foreach (var controllable in this.controllableUnits.Where(x => x.Handle == abilityOwner.Handle))
                    {
                        controllable.AddAbility(activeAbility, this.BaseHero.ComboMenus, this.BaseHero.MoveComboModeMenu);
                    }
                }
                else
                {
                    var controllable = this.controllableUnits.Find(x => x.Handle == entity.Owner.Handle);
                    controllable?.AddAbility(activeAbility, this.BaseHero.ComboMenus, this.BaseHero.MoveComboModeMenu);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        protected override void OnAbilityRemoved(Ability9 entity)
        {
            try
            {
                if (!entity.IsControllable || entity.IsFake || !entity.Owner.IsAlly(this.owner.Team)
                    || !(entity is ActiveAbility activeAbility))
                {
                    return;
                }

                foreach (var controllable in this.controllableUnits.Where(x => x.Handle == entity.Owner.Handle))
                {
                    controllable.RemoveAbility(activeAbility);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
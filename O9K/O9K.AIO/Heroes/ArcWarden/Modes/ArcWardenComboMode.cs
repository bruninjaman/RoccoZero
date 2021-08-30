﻿namespace O9K.AIO.Heroes.ArcWarden.Modes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Modes.Combo;

    using Base;

    using Core.Logger;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using CustomUnitManager;

    using Divine.Game;
    using Divine.Order;
    using Divine.Update;

    internal class ArcWardenComboMode : ComboMode
    {
        private readonly ArcWardenUnitManager arcUnitManager;

        public ArcWardenComboMode(BaseHero baseHero, IEnumerable<ComboModeMenu> comboMenus)
            : base(baseHero, comboMenus)
        {
            this.arcUnitManager = this.UnitManager as ArcWardenUnitManager;
        }

        public override void Disable()
        {
            this.UpdateHandler.IsEnabled = false;
            OrderManager.OrderAdding -= this.OnOrderAdding;

            this.ComboModeMenus.Where(x =>
                x.Value.SimplifiedName == "clonecombo").First().Key.ValueChange -= this.ToggleKeyOnValueChanged;

            foreach (var comboMenu in this.ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange -= this.KeyOnValueChanged;
            }
        }

        public override void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(this.UpdateHandler);
            OrderManager.OrderAdding -= this.OnOrderAdding;

            this.ComboModeMenus.Where(x => x.Value.SimplifiedName == "clonecombo").First().Key.ValueChange -= this.ToggleKeyOnValueChanged;

            foreach (var comboMenu in this.ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange -= this.KeyOnValueChanged;
            }
        }

        public override void Enable()
        {
            OrderManager.OrderAdding += this.OnOrderAdding;

            this.ComboModeMenus.Where(x => x.Value.SimplifiedName == "clonecombo").First().Key.ValueChange += this.ToggleKeyOnValueChanged;

            foreach (var comboMenu in this.ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange += this.KeyOnValueChanged;
            }
        }

        protected override void OnUpdate()
        {
            if (GameManager.IsPaused)
            {
                return;
            }

            try
            {
                if (this.ComboModeMenu.SimplifiedName == "clonecombo")
                {
                    if (!this.arcUnitManager.CloneControllableUnits.Any(x => x.IsValid))
                    {
                        this.TurnOffCombo();
                    }
                }

                if (this.TargetManager.HasValidTarget)
                {
                    if (this.ComboModeMenu.SimplifiedName == "clonecombo")
                    {
                        this.arcUnitManager.ExecuteCloneCombo(this.ComboModeMenu);
                    }
                    else
                    {
                        this.arcUnitManager.ExecuteCombo(this.ComboModeMenu);
                    }
                }
                else
                {
                    var cloneUnit = this.arcUnitManager.GetClone?.Owner;

                    var closestEnemyToClone = this.TargetManager.EnemyHeroes.OrderBy(x => cloneUnit?.Distance(x))
                        .FirstOrDefault();

                    var closestEnemyToMain = this.TargetManager.EnemyHeroes.OrderBy(x => this.Owner.Hero.Distance(x))
                        .FirstOrDefault();

                    if (closestEnemyToClone != null || closestEnemyToMain != null)
                    {
                        if (cloneUnit?.Distance(closestEnemyToMain) > 1500)
                        {
                            this.TargetManager.ForceSetTarget(closestEnemyToClone);
                        }
                        else
                        {
                            this.TargetManager.ForceSetTarget(closestEnemyToMain);
                        }
                    }
                }

                if (this.ComboModeMenu.SimplifiedName == "clonecombo")
                {

                    this.arcUnitManager.CloneOrbwalk(this.ComboModeMenu);
                }
                else
                {
                    this.arcUnitManager.Orbwalk(this.ComboModeMenu);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

        }

        protected override void KeyOnValueChanged(object sender, KeyEventArgs e)
        {
            if (e.NewValue)
            {
                if (this.UpdateHandler.IsEnabled)
                {
                    this.IgnoreComboEnd = true;
                }

                this.ComboModeMenu = this.ComboModeMenus[(MenuHoldKey)sender];
                this.TargetManager.TargetLocked = true;
                this.UpdateHandler.IsEnabled = true;

                this.TargetManager.ForceSetTarget(this.TargetManager.ClosestEnemyHeroToMouse());
                PushMode.Instance.TurnOffAutoPush();
            }
            else
            {
                this.TurnOffCombo();
            }
        }

        public void ToggleKeyOnValueChanged(object sender, KeyEventArgs e)
        {
            if (!e.NewValue)
            {
                return;
            }

            if (this.TargetManager.TargetLocked != true)
            {
                if (this.UpdateHandler.IsEnabled)
                {
                    this.IgnoreComboEnd = true;
                }

                this.ComboModeMenu = this.ComboModeMenus[(MenuHoldKey)sender];
                this.TargetManager.TargetLocked = true;
                this.UpdateHandler.IsEnabled = true;
                PushMode.Instance.TurnOffAutoPush();
            }
            else
            {
                this.TurnOffCombo();
            }
        }

        private bool IsCloneAlive
        {
            get
            {
                return ((ArcWardenUnitManager)this.UnitManager).CloneControllableUnits.Any();
            }
        }

        private void TurnOffCombo()
        {
            if (this.IsCloneAlive && this.ComboModeMenu.SimplifiedName != "clonecombo")
            {
                this.ComboModeMenu =  this.ComboModeMenus.Where(x =>
                    x.Value.SimplifiedName == "clonecombo").First().Value;

                return;
            }

            if (this.IgnoreComboEnd)
            {
                this.IgnoreComboEnd = false;

                return;
            }

            this.UpdateHandler.IsEnabled = false;
            this.TargetManager.TargetLocked = false;

            this.ComboEnd();
        }
    }
}
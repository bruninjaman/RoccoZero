namespace O9K.AIO.Heroes.ArcWarden.Modes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Modes.Combo;

    using Base;

    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using CustomUnitManager;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Game;
    using Divine.Helpers;
    using Divine.Order;
    using Divine.Update;

    using Draw;

    internal class ArcWardenComboMode : ComboMode
    {
        public ArcWardenComboMode(BaseHero baseHero, IEnumerable<ComboModeMenu> comboMenus)
            : base(baseHero, comboMenus)
        {
        }

        public override void Disable()
        {
            this.UpdateHandler.IsEnabled = false;
            OrderManager.OrderAdding -= this.OnOrderAdding;

            this.ComboModeMenus.First(x => x.Value.SimplifiedName == "clonecombo").Key.ValueChange -= this.ToggleKeyOnValueChanged;

            foreach (var comboMenu in this.ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange -= this.KeyOnValueChanged;
            }
        }

        public override void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(this.UpdateHandler);
            OrderManager.OrderAdding -= this.OnOrderAdding;

            this.ComboModeMenus.First(x => x.Value.SimplifiedName == "clonecombo").Key.ValueChange -= this.ToggleKeyOnValueChanged;

            foreach (var comboMenu in this.ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange -= this.KeyOnValueChanged;
            }
        }

        public override void Enable()
        {
            OrderManager.OrderAdding += this.OnOrderAdding;

            this.ComboModeMenus.First(x => x.Value.SimplifiedName == "clonecombo").Key.ValueChange += this.ToggleKeyOnValueChanged;

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

            if (this.UnitManager is ArcWardenUnitManager arcUnitManager)
            {
                try
                {
                    if (this.ComboModeMenu.SimplifiedName == "clonecombo")
                    {
                        var firstOrDefault = EntityManager9.Owner.Hero.Abilities
                                                           .FirstOrDefault(ability9 => ability9.Id == AbilityId.arc_warden_tempest_double && ability9.CanBeCasted());
                        
                        if (firstOrDefault != null && firstOrDefault.CanBeCasted())
                        {
                            firstOrDefault.BaseAbility.Cast();
                            MultiSleeper<string>.Sleep("ArcWardenTempestCast", 2000);
                        }

                        if (!arcUnitManager.CloneControllableUnits.Any() && !MultiSleeper<string>.Sleeping("ArcWardenTempestCast"))
                        {
                            this.TurnOffCombo();
                        }
                    }

                    if (this.TargetManager.HasValidTarget)
                    {
                        if (this.ComboModeMenu.SimplifiedName == "clonecombo")
                        {
                            arcUnitManager.ExecuteCloneCombo(this.ComboModeMenu);
                        }
                        else
                        {
                            arcUnitManager.ExecuteCombo(this.ComboModeMenu);
                        }
                    }

                    if (this.ComboModeMenu.SimplifiedName == "clonecombo")
                    {
                        arcUnitManager.CloneOrbwalk(this.ComboModeMenu);
                    }
                    else
                    {
                        arcUnitManager.Orbwalk(this.ComboModeMenu);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        protected override void KeyOnValueChanged(object sender, KeyEventArgs e)
        {
            if (e.NewValue)
            {
                if (this.UpdateHandler.IsEnabled && this.ComboModeMenu.SimplifiedName != "clonecombo")
                {
                    this.IgnoreComboEnd = true;
                }

                this.ComboModeMenu = this.ComboModeMenus[(MenuHoldKey)sender];
                this.TargetManager.TargetLocked = true;
                this.UpdateHandler.IsEnabled = true;
                ArcWardenPanel.unitName = this.TargetManager.Target?.BaseUnit.InternalName;
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
                ArcWardenPanel.unitName = this.TargetManager.Target?.BaseUnit.InternalName;
                PushMode.Instance.TurnOffAutoPush();
            }
            else
            {
                this.TurnOffCombo();
            }
        }

        private void TurnOffCombo()
        {
            if (this.IgnoreComboEnd)
            {
                this.IgnoreComboEnd = false;

                return;
            }

            this.UpdateHandler.IsEnabled = false;
            this.TargetManager.TargetLocked = false;
            ArcWardenPanel.unitName = null;

            this.ComboEnd();
        }
    }
}
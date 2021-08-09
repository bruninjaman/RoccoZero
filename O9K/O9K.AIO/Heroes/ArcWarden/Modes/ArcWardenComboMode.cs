namespace O9K.AIO.Heroes.ArcWarden.Modes
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

    using Draw;

    internal class ArcWardenComboMode : ComboMode
    {
        public ArcWardenComboMode(BaseHero baseHero, IEnumerable<ComboModeMenu> comboMenus)
            : base(baseHero, comboMenus)
        {
        }

        public override void Disable()
        {
            UpdateHandler.IsEnabled = false;
            OrderManager.OrderAdding -= OnOrderAdding;

            ComboModeMenus.Where(x =>
                    x.Value.SimplifiedName == "clonecombo").First().Key.ValueChange -=
                ToggleKeyOnValueChanged;

            foreach (var comboMenu in ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange -= KeyOnValueChanged;
            }
        }

        public override void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(UpdateHandler);
            OrderManager.OrderAdding -= OnOrderAdding;

            ComboModeMenus.Where(x => x.Value.SimplifiedName == "clonecombo").First().Key.ValueChange -=
                ToggleKeyOnValueChanged;

            foreach (var comboMenu in ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange -= KeyOnValueChanged;
            }
        }

        public override void Enable()
        {
            OrderManager.OrderAdding += OnOrderAdding;

            ComboModeMenus.Where(x => x.Value.SimplifiedName == "clonecombo").First().Key.ValueChange +=
                ToggleKeyOnValueChanged;

            foreach (var comboMenu in ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange += KeyOnValueChanged;
            }
        }

        protected override void OnUpdate()
        {
            if (GameManager.IsPaused)
            {
                return;
            }

            var arcUnitManager = UnitManager as ArcWardenUnitManager;

            if (arcUnitManager != null)
            {
                try
                {
                    if (ComboModeMenu.SimplifiedName == "clonecombo")
                    {
                        if (!arcUnitManager.CloneControllableUnits.Any(x => x.IsValid))
                        {
                            TurnOffCombo();
                        }
                    }

                    if (TargetManager.HasValidTarget)
                    {
                        if (ComboModeMenu.SimplifiedName == "clonecombo")
                        {
                            arcUnitManager.ExecuteCloneCombo(ComboModeMenu);
                        }
                        else
                        {
                            arcUnitManager.ExecuteCombo(ComboModeMenu);
                        }
                    }

                    if (ComboModeMenu.SimplifiedName == "clonecombo")
                    {
                        arcUnitManager.CloneOrbwalk(ComboModeMenu);
                    }
                    else
                    {
                        arcUnitManager.Orbwalk(ComboModeMenu);
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
                if (UpdateHandler.IsEnabled && ComboModeMenu.SimplifiedName != "clonecombo")
                {
                    IgnoreComboEnd = true;
                }

                ComboModeMenu = ComboModeMenus[(MenuHoldKey)sender];
                TargetManager.TargetLocked = true;
                UpdateHandler.IsEnabled = true;
                ArcWardenDrawPanel.unitName = TargetManager.Target?.BaseUnit.InternalName;
                PushMode.Instance.TurnOffAutoPush();
            }
            else
            {
                TurnOffCombo();
            }
        }

        public void ToggleKeyOnValueChanged(object sender, KeyEventArgs e)
        {
            if (!e.NewValue)
            {
                return;
            }

            if (TargetManager.TargetLocked != true)
            {
                if (UpdateHandler.IsEnabled)
                {
                    IgnoreComboEnd = true;
                }

                ComboModeMenu = ComboModeMenus[(MenuHoldKey)sender];
                TargetManager.TargetLocked = true;
                UpdateHandler.IsEnabled = true;
                ArcWardenDrawPanel.unitName = TargetManager.Target?.BaseUnit.InternalName;
                PushMode.Instance.TurnOffAutoPush();
            }
            else
            {
                TurnOffCombo();
            }
        }

        private void TurnOffCombo()
        {
            if (IgnoreComboEnd)
            {
                IgnoreComboEnd = false;

                return;
            }

            UpdateHandler.IsEnabled = false;
            TargetManager.TargetLocked = false;
            ArcWardenDrawPanel.unitName = null;

            ComboEnd();
        }
    }
}
namespace O9K.AIO.Heroes.ArcWarden.ArcWardenComboMode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using Core.Logger;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using CustomUnitManager;

    using Divine.Game;
    using Divine.Numerics;
    using Divine.Order;
    using Divine.Renderer;
    using Divine.Update;

    using Modes.Combo;

    internal class ArcWardenComboMode : ComboMode
    {
        private MenuSlider drawXStatusSlider;
        private MenuSlider drawYStatusSlider;
        public ArcWardenComboMode(BaseHero baseHero, IEnumerable<ComboModeMenu> comboMenus)
            : base(baseHero, comboMenus)
        {
        }

        public override void Disable()
        {
            this.UpdateHandler.IsEnabled = false;
            OrderManager.OrderAdding -= this.OnOrderAdding;

            ComboModeMenus.Where(x =>
                    x.Value.SimplifiedName == "clonecombo").First().Key.ValueChange -=
                this.ToggleKeyOnValueChanged;


            foreach (var comboMenu in this.ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange -= this.KeyOnValueChanged;
            }
        }

        public override void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(this.UpdateHandler);
            OrderManager.OrderAdding -= this.OnOrderAdding;


            ComboModeMenus.Where(x => x.Value.SimplifiedName == "clonecombo").First().Key.ValueChange -=
                this.ToggleKeyOnValueChanged;
            foreach (var comboMenu in this.ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange -= this.KeyOnValueChanged;
            }
        }


        public override void Enable()
        {
            // Draw 
            var drawMenu = Menu.RootMenu.Add(new Menu("Draw combo status"));

            drawXStatusSlider = drawMenu.Add(new MenuSlider("X pos", 0, 0, 3000));
            drawYStatusSlider = drawMenu.Add(new MenuSlider("Y pos", 0, 0, 3000));

            RendererManager.Draw += this.DrawComboStatus;

            OrderManager.OrderAdding += this.OnOrderAdding;

            ComboModeMenus.Where(x => x.Value.SimplifiedName == "clonecombo").First().Key.ValueChange +=
                this.ToggleKeyOnValueChanged;

            foreach (var comboMenu in this.ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange += this.KeyOnValueChanged;
            }
        }
        private void DrawComboStatus()
        {
            if (this.ComboModeMenu?.SimplifiedName == "clonecombo" && this.TargetManager.HasValidTarget && this.UpdateHandler.IsEnabled)
            {
                RendererManager.DrawText("Clone combo enabled", new Vector2(drawXStatusSlider, drawYStatusSlider), Color.Red, 20);
            }
        }

        protected override void OnUpdate()
        {
            if (GameManager.IsPaused)
            {
                return;
            }

            var arcUnitManager = this.UnitManager as ArcWardenUnitManager;
            if (arcUnitManager != null)
            {
                try
                {
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
            }
            else
            {
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
            }
            else
            {
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
}
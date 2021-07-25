using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Game;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Order;
using Divine.Order.EventArgs;
using Divine.Order.Orders.Components;
using Divine.Renderer;
using Divine.Update;
using O9K.AIO.Heroes.ArcWarden.CustomUnitManager;
using O9K.AIO.Heroes.Base;
using O9K.AIO.Modes.Base;
using O9K.AIO.Modes.Combo;
using O9K.AIO.UnitManager;
using O9K.Core.Entities.Abilities.Base.Components;
using O9K.Core.Logger;
using O9K.Core.Managers.Entity;
using O9K.Core.Managers.Menu.EventArgs;
using MenuHoldKey = O9K.Core.Managers.Menu.Items.MenuHoldKey;

namespace O9K.AIO.Heroes.ArcWarden.ArcWardenComboMode
{
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
            OrderManager.OrderAdding += this.OnOrderAdding;

            ComboModeMenus.Where(x => x.Value.SimplifiedName == "clonecombo").First().Key.ValueChange +=
                this.ToggleKeyOnValueChanged;
            
            foreach (var comboMenu in this.ComboModeMenus.Where(x => x.Value.SimplifiedName != "clonecombo"))
            {
                comboMenu.Key.ValueChange += this.KeyOnValueChanged;
                
            }
        }

        public override void OnUpdate()
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

        public override void KeyOnValueChanged(object sender, KeyEventArgs e)
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
namespace O9K.AIO.Heroes.Base
{
    using System;
    using System.Collections.Generic;

    using Core.Entities.Heroes;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Context;
    using Core.Managers.Entity;
    using Core.Managers.Menu.EventArgs;

    using Divine.Update;

    using FailSafe;

    using KillStealer;

    using Menu;

    using Modes.Combo;
    using Modes.MoveCombo;

    using ShieldBreaker;

    using TargetManager;

    using UnitManager;

    internal class BaseHero : IDisposable
    {
        protected IComboMode Combo { get; set; } 

        protected MoveComboMode moveCombo { get; set; } 

        public BaseHero()
        {
            this.Owner = EntityManager9.Owner;

            this.Menu = new MenuManager(this.Owner, Context9.MenuManager);

            this.TargetManager = new TargetManager(this.Menu);

            this.KillSteal = new KillSteal(this);
            this.FailSafe = new FailSafe(this);
            this.ShieldBreaker = new ShieldBreaker(this);

            this.MoveComboModeMenu = new MoveComboModeMenu(this.Menu.RootMenu, "Move");
            this.ComboMenus.Add(new ComboModeMenu(this.Menu.RootMenu, "Harass"));

            // ReSharper disable once VirtualMemberCallInConstructor
            this.CreateComboMenus();

            this.ShieldBreaker.AddComboMenu(this.ComboMenus);

            // ReSharper disable once VirtualMemberCallInConstructor
            this.CreateUnitManager();

            this.ShieldBreaker.UnitManager = this.UnitManager;
            
            
            // ReSharper disable once VirtualMemberCallInConstructor
            this.CreateComboMode(this, this.ComboMenus);
            
            this.moveCombo = new MoveComboMode(this, this.MoveComboModeMenu);

            UpdateManager.BeginInvoke(1000, () => this.Menu.Enabled.ValueChange += this.EnabledOnValueChange);
        }

        public virtual void CreateComboMode(BaseHero baseHero, List<ComboModeMenu> comboMenus)
        {
            this.Combo = new ComboMode(this, this.ComboMenus);
        }

        public MultiSleeper AbilitySleeper { get; } = new MultiSleeper();

        public List<ComboModeMenu> ComboMenus { get; } = new List<ComboModeMenu>();

        public FailSafe FailSafe { get; }

        public KillSteal KillSteal { get; }

        public MenuManager Menu { get; }

        public MoveComboModeMenu MoveComboModeMenu { get; }

        public MultiSleeper OrbwalkSleeper { get; } = new MultiSleeper();

        public Owner Owner { get; }

        public ShieldBreaker ShieldBreaker { get; }

        public TargetManager TargetManager { get; }

        public IUnitManager UnitManager { get; protected set; }

        public virtual void CreateUnitManager()
        {
            this.UnitManager = new UnitManager(this);
        }

        public virtual void Dispose()
        {
            this.Menu.Enabled.ValueChange -= this.EnabledOnValueChange;

            this.ComboMenus.Clear();
            this.Combo.Dispose();
            this.moveCombo.Dispose();
            this.KillSteal.Dispose();
            this.FailSafe.Dispose();
            this.ShieldBreaker.Dispose();
            this.UnitManager.Dispose();
            this.Menu.Dispose();
        }

        protected virtual void CreateComboMenus()
        {
            this.ComboMenus.Add(new ComboModeMenu(this.Menu.RootMenu, "Combo"));
            this.ComboMenus.Add(new ComboModeMenu(this.Menu.RootMenu, "Alternative combo"));
        }

        protected virtual void DisableCustomModes()
        {
        }

        protected virtual void EnableCustomModes()
        {
        }

        protected void EnabledOnValueChange(object sender, SwitcherEventArgs e)
        {
            try
            {
                if (e.NewValue)
                {
                    this.TargetManager.Enable();
                    this.KillSteal.Enable();
                    this.FailSafe.Enable();
                    this.ShieldBreaker.Enable();
                    this.UnitManager.Enable();
                    this.Combo.Enable();
                    this.moveCombo.Enable();
                    this.EnableCustomModes();
                }
                else
                {
                    this.DisableCustomModes();
                    this.TargetManager.Disable();
                    this.KillSteal.Disable();
                    this.FailSafe.Disable();
                    this.ShieldBreaker.Disable();
                    this.UnitManager.Disable();
                    this.Combo.Disable();
                    this.moveCombo.Disable();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
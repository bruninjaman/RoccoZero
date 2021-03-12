namespace O9K.AIO.Modes.KeyPress
{
    using System;

    using Base;

    using Core.Logger;

    using Divine;

    using Heroes.Base;

    using UnitManager;

    using KeyEventArgs = Core.Managers.Menu.EventArgs.KeyEventArgs;

    internal abstract class KeyPressMode : BaseMode
    {
        private readonly KeyPressModeMenu menu;

        protected KeyPressMode(BaseHero baseHero, KeyPressModeMenu menu)
            : base(baseHero)
        {
            this.UnitManager = baseHero.UnitManager;
            this.menu = menu;

            this.UpdateHandler = UpdateManager.CreateIngameUpdate(0, false, this.OnUpdate);
        }

        protected bool LockTarget { get; set; } = true;

        protected UnitManager UnitManager { get; }

        protected UpdateHandler UpdateHandler { get; }

        public virtual void Disable()
        {
            this.UpdateHandler.IsEnabled = false;
            this.menu.Key.ValueChange -= this.KeyOnValueChanged;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.menu.Key.ValueChange -= this.KeyOnValueChanged;
        }

        public virtual void Enable()
        {
            this.menu.Key.ValueChange += this.KeyOnValueChanged;
        }

        protected abstract void ExecuteCombo();

        protected virtual void KeyOnValueChanged(object sender, KeyEventArgs e)
        {
            if (e.NewValue)
            {
                if (this.LockTarget)
                {
                    this.TargetManager.TargetLocked = true;
                }

                this.UpdateHandler.IsEnabled = true;
            }
            else
            {
                this.UpdateHandler.IsEnabled = false;

                if (this.LockTarget)
                {
                    this.TargetManager.TargetLocked = false;
                }
            }
        }

        protected void OnUpdate()
        {
            if (GameManager.IsPaused)
            {
                return;
            }

            try
            {
                this.ExecuteCombo();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
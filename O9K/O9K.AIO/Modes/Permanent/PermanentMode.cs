namespace O9K.AIO.Modes.Permanent
{
    using System;

    using Base;

    using Core.Logger;
    using Core.Managers.Menu.EventArgs;

    using Divine;

    using Heroes.Base;

    using UnitManager;

    internal abstract class PermanentMode : BaseMode
    {
        protected readonly UpdateHandler Handler;

        private readonly PermanentModeMenu menu;

        protected PermanentMode(BaseHero baseHero, PermanentModeMenu menu)
            : base(baseHero)
        {
            this.UnitManager = baseHero.UnitManager;
            this.menu = menu;

            this.Handler = UpdateManager.CreateIngameUpdate(0, menu.Enabled, this.OnUpdate);
        }

        protected UnitManager UnitManager { get; }

        public virtual void Disable()
        {
            this.Handler.IsEnabled = false;
            this.menu.Enabled.ValueChange -= this.EnabledOnValueChanged;
        }

        public override void Dispose()
        {
            base.Dispose();
            UpdateManager.DestroyIngameUpdate(this.Handler);
            this.menu.Enabled.ValueChange -= this.EnabledOnValueChanged;
        }

        public virtual void Enable()
        {
            this.menu.Enabled.ValueChange += this.EnabledOnValueChanged;
        }

        protected abstract void Execute();

        private void EnabledOnValueChanged(object sender, SwitcherEventArgs e)
        {
            this.Handler.IsEnabled = e.NewValue;
        }

        private void OnUpdate()
        {
            if (GameManager.IsPaused)
            {
                return;
            }

            try
            {
                this.Execute();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
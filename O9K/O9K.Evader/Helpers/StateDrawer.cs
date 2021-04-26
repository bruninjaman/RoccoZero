namespace O9K.Evader.Helpers
{
    using System;

    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Renderer.Utils;

    using Divine;

    using Metadata;

    using Pathfinder;

    using Settings;

    using SharpDX;

    internal class StateDrawer : IEvaderService
    {
        private readonly HotkeysMenu hotkeysMenu;

        private readonly RectangleF startPosition;

        private readonly Sleeper stateSleeper = new Sleeper(10);

        private readonly float textSize;

        private Pathfinder.EvadeMode pathfinderMode;

        private bool showBkb;

        private bool showProactive;

        public StateDrawer(IMainMenu menu)
        {
            this.hotkeysMenu = menu.Hotkeys;

            this.textSize = 22 * Hud.Info.ScreenRatio;
            this.startPosition = new Rectangle9(0, Hud.Info.ScreenSize.Y * 0.05f, Hud.Info.ScreenSize.X * 0.995f, this.textSize);
        }

        public LoadOrder LoadOrder { get; } = LoadOrder.StateDrawer;

        public void Activate()
        {
            this.hotkeysMenu.BkbEnabled.ValueChange += this.BkbEnabledOnValueChanged;
            this.hotkeysMenu.PathfinderMode.ValueChange += this.PathfinderModeOnValueChanged;
            this.hotkeysMenu.ProactiveEvade.ValueChange += this.ProactiveEvadeOnValueChange;

            RendererManager.Draw += this.RendererOnDraw;
        }

        public void Dispose()
        {
            this.hotkeysMenu.BkbEnabled.ValueChange -= this.BkbEnabledOnValueChanged;
            this.hotkeysMenu.PathfinderMode.ValueChange -= this.PathfinderModeOnValueChanged;
            this.hotkeysMenu.ProactiveEvade.ValueChange -= this.ProactiveEvadeOnValueChange;

            RendererManager.Draw -= this.RendererOnDraw;
        }

        private void BkbEnabledOnValueChanged(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            this.stateSleeper.Sleep(5);
            this.showBkb = e.NewValue;
        }

        private void PathfinderModeOnValueChanged(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            this.stateSleeper.Sleep(5);

            if (!e.NewValue)
            {
                return;
            }

            if ((int)this.pathfinderMode >= Enum.GetNames(typeof(Pathfinder.EvadeMode)).Length - 1)
            {
                this.pathfinderMode = 0;
            }
            else
            {
                this.pathfinderMode++;
            }
        }

        private void ProactiveEvadeOnValueChange(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            this.stateSleeper.Sleep(5);
            this.showProactive = e.NewValue;
        }

        private void RendererOnDraw()
        {
            try
            {
                if (!this.hotkeysMenu.DrawState && !this.stateSleeper.IsSleeping)
                {
                    return;
                }

                var position = this.startPosition;

                if (this.showProactive)
                {
                    RendererManager.DrawText("Evader (Proactive)", position, Color.OrangeRed, FontFlags.Right, this.textSize);
                }
                else
                {
                    RendererManager.DrawText("Evader", position, Color.LawnGreen, FontFlags.Right, this.textSize);
                }

                position.Y += this.textSize;

                switch (this.pathfinderMode)
                {
                    case Pathfinder.EvadeMode.All:
                        RendererManager.DrawText("Dodge", position, Color.LawnGreen, FontFlags.Right, this.textSize);
                        break;
                    case Pathfinder.EvadeMode.Disables:
                        RendererManager.DrawText("Dodge (Disables)", position, Color.OrangeRed, FontFlags.Right, this.textSize);
                        break;
                    case Pathfinder.EvadeMode.None:
                        RendererManager.DrawText("Dodge (None)", position, Color.Red, FontFlags.Right, this.textSize);
                        break;
                }

                position.Y += this.textSize;

                RendererManager.DrawText("BKB", position, this.showBkb ? Color.LawnGreen : Color.Red, FontFlags.Right, this.textSize);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
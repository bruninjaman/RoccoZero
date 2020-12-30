namespace O9K.Core.Managers.Menu
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Divine;

    using Items;

    using Logger;

    public sealed class MenuManager9 : IMenuManager9, IDisposable
    {
        private readonly MainMenu mainMenu;

        private readonly Key menuHoldKey;

        private readonly Key menuToggleKey;

        private bool menuVisible;

        public MenuManager9()
        {
            this.mainMenu = new MainMenu();

            /*if (SandboxConfig.Config.HotKeys.TryGetValue("Menu", out var menuKey))
            {
                this.menuHoldKey = KeyInterop.KeyFromVirtualKey(menuKey);
            }

            if (SandboxConfig.Config.HotKeys.TryGetValue("MenuToggle", out menuKey))
            {
                this.menuToggleKey = KeyInterop.KeyFromVirtualKey(menuKey);
            }*/

            InputManager.KeyDown += this.OnKeyDown;
        }

        public void AddRootMenu(Menu menu)
        {
            try
            {
                this.mainMenu.Add(menu);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void Dispose()
        {
            InputManager.MouseKeyDown -= this.OnMouseKeyDown;
            InputManager.MouseKeyUp -= this.OnMouseKeyUp;
            InputManager.MouseMove -= this.OnMouseMove;
            InputManager.MouseWheel -= this.OnMouseWheel;
            InputManager.KeyDown -= this.OnKeyDown;
            InputManager.KeyUp -= this.OnKeyUp;
            RendererManager.Draw -= this.OnDraw;
        }

        public void RemoveRootMenu(Menu menu)
        {
            try
            {
                this.mainMenu.Remove(menu);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnDraw()
        {
            try
            {
                this.mainMenu.DrawMenu();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == this.menuToggleKey)
            {
                if (this.menuVisible)
                {
                    this.Unsubscribe();
                    this.SaveMenu();
                }
                else
                {
                    this.Subscribe();
                }
            }
            else if (!this.menuVisible && e.Key == this.menuHoldKey)
            {
                this.Subscribe();
            }
        }

        private void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key != this.menuHoldKey)
            {
                return;
            }

            this.Unsubscribe();
            this.SaveMenu();
        }

        private void OnMouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            if (this.mainMenu.OnMousePress(e.Position))
            {
                e.Process = false;
            }
        }

        private void OnMouseKeyUp(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            if (this.mainMenu.OnMouseRelease(e.Position))
            {
                e.Process = false;
            }
        }

        private void OnMouseMove(MouseMoveEventArgs e)
        {
            try
            {
                if (this.mainMenu.OnMouseMove(e.Position))
                {
                    e.Process = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnMouseWheel(MouseWheelEventArgs e)
        {
            try
            {
                if (this.mainMenu.OnMouseWheel(e.Position, e.Up))
                {
                    e.Process = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void SaveMenu()
        {
            Task.Run(
                () =>
                    {
                        try
                        {
                            lock (this.mainMenu)
                            {
                                this.mainMenu.Save();
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                    });
        }

        private void Subscribe()
        {
            InputManager.MouseKeyDown += this.OnMouseKeyDown;
            InputManager.MouseKeyUp += this.OnMouseKeyUp;
            InputManager.MouseMove += this.OnMouseMove;
            InputManager.MouseWheel += this.OnMouseWheel;
            InputManager.KeyUp += this.OnKeyUp;
            RendererManager.Draw += this.OnDraw;
            this.menuVisible = true;
        }

        private void Unsubscribe()
        {
            InputManager.MouseKeyDown -= this.OnMouseKeyDown;
            InputManager.MouseKeyUp -= this.OnMouseKeyUp;
            InputManager.MouseMove -= this.OnMouseMove;
            InputManager.MouseWheel -= this.OnMouseWheel;
            InputManager.KeyUp -= this.OnKeyUp;
            RendererManager.Draw -= this.OnDraw;
            this.menuVisible = false;
        }
    }
}
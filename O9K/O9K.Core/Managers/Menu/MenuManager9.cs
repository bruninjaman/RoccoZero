namespace O9K.Core.Managers.Menu;

using System;
using System.Threading.Tasks;

using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Menu;
using Divine.Renderer;
using Divine.Service;
using Divine.Update;

using Items;

using Logger;

public sealed class MenuManager9 : IMenuManager9, IDisposable
{
    private readonly MainMenu mainMenu;

    private bool menuVisible;

    public MenuManager9()
    {
        this.mainMenu = new MainMenu();

        Divine.Service.Language? lastLanguage = null;

        UpdateManager.CreateIngameUpdate(2000, () =>
        {
            if (lastLanguage != null && lastLanguage == DivineService.Language)
            {
                return;
            }

            lastLanguage = DivineService.Language;

            switch (lastLanguage)
            {
                case Divine.Service.Language.Ru:
                    this.mainMenu.SetLanguage(Lang.Ru);
                    break;
                case Divine.Service.Language.Cn:
                    this.mainMenu.SetLanguage(Lang.Cn);
                    break;
                default:
                    this.mainMenu.SetLanguage(Lang.En);
                    break;
            }

            this.mainMenu.CalculateSize();
            this.mainMenu.CalculateWidth(true);
        });

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

        SaveMenu();
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
        if (e.Key == MenuManager.MenuToggleKey.Key)
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
        else if (!this.menuVisible && e.Key == MenuManager.MenuHoldKey.Key)
        {
            this.Subscribe();
        }
    }

    private void OnKeyUp(KeyEventArgs e)
    {
        if (e.Key != MenuManager.MenuHoldKey.Key)
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
        Task.Run(() =>
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
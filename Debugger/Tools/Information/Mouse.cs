namespace Debugger.Tools.Information;

using System.Windows;

using Debugger.Menus;

using Divine.Game;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Update;

using Logger;

internal class Mouse : IDebuggerTool
{
    private const uint WM_LBUTTONDOWN = 0x0201;

    private MenuSwitcher copyPosition;

    private readonly ILog log;

    private IMainMenu mainMenu;

    private Menu menu;

    private MenuSwitcher showMousePosition;

    public Mouse(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
    }

    public int LoadPriority { get; } = 77;

    public void Activate()
    {
        this.menu = this.mainMenu.InformationMenu.CreateMenu("Mouse");

        this.showMousePosition = this.menu.CreateSwitcher("Show mouse position", false);
        this.showMousePosition.ValueChanged += this.ShowMousePositionOnPropertyChanged;

        this.copyPosition = this.menu.CreateSwitcher("Copy position on click", true);
        this.copyPosition.ValueChanged += this.CopyPositionOnPropertyChanged;

        this.ShowMousePositionOnPropertyChanged(null, null);
        this.CopyPositionOnPropertyChanged(null, null);
    }

    public void Dispose()
    {
        this.showMousePosition.ValueChanged -= this.ShowMousePositionOnPropertyChanged;
        this.copyPosition.ValueChanged -= this.CopyPositionOnPropertyChanged;
        RendererManager.Draw -= this.DrawingOnDraw;
        InputManager.WindowProc -= this.GameOnWndProc;
    }

    private void CopyPositionOnPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.copyPosition && this.showMousePosition)
            {
                InputManager.WindowProc += this.GameOnWndProc;
            }
            else
            {
                InputManager.WindowProc -= this.GameOnWndProc;
            }
        });
    }

    private void DrawingOnDraw()
    {
        var pos = GameManager.MousePosition;

        RendererManager.DrawText(
            pos.ToCopyFormat(),
            GameManager.MouseScreenPosition + new Vector2(35, 0),
            Color.White,
            "Arial",
            20);
    }

    private void GameOnWndProc(WindowProcEventArgs e)
    {
        if (e.Msg == WM_LBUTTONDOWN && !this.log.IsMouseUnderLog())
        {
            Clipboard.SetText(GameManager.MousePosition.ToCopyFormat());
        }
    }

    private void ShowMousePositionOnPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.showMousePosition)
            {
                RendererManager.Draw += this.DrawingOnDraw;
            }
            else
            {
                RendererManager.Draw -= this.DrawingOnDraw;
            }

            this.CopyPositionOnPropertyChanged(null, null);
        });
    }
}
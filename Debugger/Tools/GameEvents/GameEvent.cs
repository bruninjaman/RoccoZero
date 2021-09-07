namespace Debugger.Tools.GameEvents;

using System.Collections.Generic;

using Debugger.Menus;

using Divine.Game;
using Divine.Game.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Update;

using Logger;

internal class GameEvent : IDebuggerTool
{
    private readonly HashSet<string> ignored = new HashSet<string>
    {
        "dota_action_success"
    };

    private MenuSwitcher enabled;

    private MenuSwitcher ignoreUseless;

    private IMainMenu mainMenu;

    private readonly ILog log;

    private Menu menu;

    public GameEvent(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
    }

    public int LoadPriority { get; } = 85;

    public void Activate()
    {
        this.menu = this.mainMenu.GameEventsMenu.CreateMenu("Game event");

        this.enabled = this.menu.CreateSwitcher("Enabled", false);
        this.enabled.SetTooltip("Game.OnGameEvent");
        this.enabled.ValueChanged += this.EnabledOnPropertyChanged;

        this.ignoreUseless = this.menu.CreateSwitcher("Ignore useless", true);

        this.EnabledOnPropertyChanged(null, null);
    }

    public void Dispose()
    {
        this.enabled.ValueChanged -= this.EnabledOnPropertyChanged;
        GameManager.GameEvent -= this.GameOnGameEvent;
    }

    private void EnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                GameManager.GameEvent += this.GameOnGameEvent;
            }
            else
            {
                this.menu.RemoveAsterisk();
                GameManager.GameEvent -= this.GameOnGameEvent;
            }
        });
    }

    private void GameOnGameEvent(GameEventEventArgs e)
    {
        if (!this.IsValid(e))
        {
            return;
        }

        var item = new LogItem(LogType.GameEvent, Color.Yellow, "Game event");

        item.AddLine("Name: " + e.GameEvent.Name, e.GameEvent.Name);

        this.log.Display(item);
    }

    private bool IsValid(GameEventEventArgs e)
    {
        if (this.ignoreUseless && this.ignored.Contains(e.GameEvent.Name))
        {
            return false;
        }

        return true;
    }
}
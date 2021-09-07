namespace Debugger.Tools.Information;

using System.Linq;

using Debugger.Menus;

using Divine.Entity;
using Divine.Entity.Entities.Players;
using Divine.Entity.Entities.Units;
using Divine.Game;
using Divine.Game.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Update;

using Logger;

internal class Modifiers : IDebuggerTool
{
    private readonly Player player;

    private MenuSwitcher autoUpdate;

    private MenuSwitcher information;

    private uint lastUnitInfo;

    private readonly ILog log;

    private IMainMenu mainMenu;

    private Menu menu;

    private MenuSwitcher showElapsedTime;

    private MenuSwitcher showHidden;

    private MenuSwitcher showRemainingTime;

    private MenuSwitcher showTextureName;

    public Modifiers(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
        this.player = EntityManager.LocalPlayer;
    }

    public int LoadPriority { get; } = 79;

    public void Activate()
    {
        this.menu = this.mainMenu.InformationMenu.CreateMenu("Modifiers");

        this.information = this.menu.CreateSwitcher("Get", false);
        this.information.ValueChanged += this.InformationOnPropertyChanged;

        this.autoUpdate = this.menu.CreateSwitcher("Auto update", false);
        this.autoUpdate.ValueChanged += this.AutoUpdateOnPropertyChanged;

        this.showHidden = this.menu.CreateSwitcher("Show hidden", false);
        this.showTextureName = this.menu.CreateSwitcher("Show texture name", false);
        this.showRemainingTime = this.menu.CreateSwitcher("Show remaining time", false);
        this.showElapsedTime = this.menu.CreateSwitcher("Show elapsed time", false);

        this.AutoUpdateOnPropertyChanged(null, null);
    }

    public void Dispose()
    {
        this.information.ValueChanged -= this.InformationOnPropertyChanged;
        this.autoUpdate.ValueChanged -= this.AutoUpdateOnPropertyChanged;
        GameManager.GameEvent -= this.GameOnGameEvent;
    }

    private void AutoUpdateOnPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.autoUpdate)
            {
                this.menu.AddAsterisk();
                GameManager.GameEvent += this.GameOnGameEvent;
                this.InformationOnPropertyChanged(null, null);
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
        if (e.GameEvent.Name != "dota_player_update_selected_unit" && e.GameEvent.Name != "dota_player_update_query_unit")
        {
            return;
        }

        var unit = (this.player.QueryUnit ?? this.player.SelectedUnits.FirstOrDefault()) as Unit;
        if (unit?.IsValid != true)
        {
            return;
        }

        if (unit.Handle == this.lastUnitInfo)
        {
            return;
        }

        this.InformationOnPropertyChanged(null, null);
    }

    private void InformationOnPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        if (e.Value == information)
        {
            return;
        }

        UpdateManager.BeginInvoke(() =>
        {
            var unit = (this.player.QueryUnit ?? this.player.SelectedUnits.FirstOrDefault()) as Unit;
            if (unit?.IsValid != true)
            {
                return;
            }

            this.lastUnitInfo = unit.Handle;

            var item = new LogItem(LogType.Modifier, Color.PaleGreen, "Modifiers information");

            item.AddLine("Unit name: " + unit.Name, unit.Name);
            item.AddLine("Unit network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("Unit classID: " + unit.ClassId, unit.ClassId);

            foreach (var modifier in unit.Modifiers)
            {
                if (!this.showHidden && modifier.IsHidden)
                {
                    continue;
                }

                var modifierItem = new LogItem(LogType.Modifier, Color.PaleGreen);

                modifierItem.AddLine("Name: " + modifier.Name, modifier.Name);

                if (this.showTextureName)
                {
                    modifierItem.AddLine("Texture name: " + modifier.TextureName, modifier.TextureName);
                }

                if (this.showHidden)
                {
                    modifierItem.AddLine("Is hidden: " + modifier.IsHidden, modifier.IsHidden);
                }

                if (this.showElapsedTime)
                {
                    modifierItem.AddLine("Elapsed time: " + modifier.ElapsedTime, modifier.ElapsedTime);
                }

                if (this.showRemainingTime)
                {
                    modifierItem.AddLine("Remaining time: " + modifier.RemainingTime, modifier.RemainingTime);
                }

                this.log.Display(modifierItem);
            }

            this.log.Display(item);
        });
    }
}
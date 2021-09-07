namespace Debugger.Tools.Information;

using System.Linq;

using Debugger.Menus;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Players;
using Divine.Entity.Entities.Units;
using Divine.Game;
using Divine.Game.EventArgs;
using Divine.Helpers;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Update;

using Logger;

internal class Units : IDebuggerTool
{
    private readonly Player player;

    private MenuSwitcher autoUpdate;

    private MenuSwitcher information;

    private uint lastUnitInfo;

    private readonly ILog log;

    private IMainMenu mainMenu;

    private Menu menu;

    private MenuSwitcher showItemInfo;

    private MenuSwitcher showLevel;

    private MenuSwitcher showModifierInfo;

    private MenuSwitcher showsAbilityInfo;

    private MenuSwitcher showsHpMp;

    private MenuSwitcher showState;

    private MenuSwitcher showTeam;

    private MenuSwitcher showVision;

    public Units(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
        this.player = EntityManager.LocalPlayer;
    }

    public int LoadPriority { get; } = 78;

    public void Activate()
    {
        this.menu = this.mainMenu.InformationMenu.CreateMenu("Units");

        this.information = this.menu.CreateSwitcher("Get", false);
        this.information.ValueChanged += this.InformationOnPropertyChanged;

        this.autoUpdate = this.menu.CreateSwitcher("Auto update", false);
        this.autoUpdate.ValueChanged += this.AutoUpdateOnPropertyChanged;

        this.showTeam = this.menu.CreateSwitcher("Show team", true);
        this.showLevel = this.menu.CreateSwitcher("Show level", true);
        this.showsHpMp = this.menu.CreateSwitcher("Show hp/mp", true);
        this.showVision = this.menu.CreateSwitcher("Show vision", true);
        this.showState = this.menu.CreateSwitcher("Show state", true);
        this.showsAbilityInfo = this.menu.CreateSwitcher("Show ability information", false);
        this.showItemInfo = this.menu.CreateSwitcher("Show item information", false);
        this.showModifierInfo = this.menu.CreateSwitcher("Show modifier information", false);

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

    private void GameOnGameEvent(GameEventEventArgs args)
    {
        if (args.GameEvent.Name != "dota_player_update_selected_unit" && args.GameEvent.Name != "dota_player_update_query_unit")
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

            var item = new LogItem(LogType.Unit, Color.PaleGreen, "Unit information");

            item.AddLine("Unit name: " + unit.Name, unit.Name);
            item.AddLine("Unit network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("Unit classID: " + unit.ClassId, unit.ClassId);
            var localizeName = LocalizationHelper.LocalizeName(unit);
            item.AddLine("Unit display name: " + localizeName, localizeName);
            item.AddLine("Unit type: " + unit.UnitType, unit.UnitType);
            item.AddLine("Unit position: " + unit.Position, unit.Position);
            if (this.showLevel)
            {
                item.AddLine("Unit level: " + unit.Level, unit.Level);
            }

            if (this.showTeam)
            {
                item.AddLine("Unit team: " + unit.Team, unit.Team);
            }

            if (this.showsHpMp)
            {
                item.AddLine("Unit health: " + unit.Health + "/" + unit.MaximumHealth);
                item.AddLine("Unit mana: " + (int)unit.Mana + "/" + (int)unit.MaximumMana);
            }

            item.AddLine("Unit attack capability: " + unit.AttackCapability, unit.AttackCapability);
            if (this.showVision)
            {
                item.AddLine("Unit vision: " + unit.DayVision + "/" + unit.NightVision);
            }

            if (this.showState)
            {
                item.AddLine("Unit state: " + unit.UnitState, unit.UnitState);
            }

            if (this.showsAbilityInfo)
            {
                item.AddLine("Abilities =>");
                item.AddLine("  Talents count: " + unit.Spellbook.Spells.Count(x => x.Name.StartsWith("special_")));
                item.AddLine(
                    "  Active spells count: " + unit.Spellbook.Spells.Count(
                        x => !x.Name.StartsWith("special_") && x.AbilityBehavior != AbilityBehavior.Passive));
                item.AddLine(
                    "  Passive spells count: " + unit.Spellbook.Spells.Count(
                        x => !x.Name.StartsWith("special_") && x.AbilityBehavior == AbilityBehavior.Passive));
            }

            if (this.showItemInfo && unit.HasInventory)
            {
                item.AddLine("Items =>");
                item.AddLine("  Inventory Items count: " + unit.Inventory.Items.Count());
                item.AddLine("  Backpack Items count: " + unit.Inventory.BackpackItems.Count());
                item.AddLine("  Stash Items count: " + unit.Inventory.StashItems.Count());
            }

            if (this.showModifierInfo)
            {
                item.AddLine("Modifiers =>");
                item.AddLine("  Active modifiers count: " + unit.Modifiers.Count(x => !x.IsHidden));
                item.AddLine("  Hidden modifiers count: " + unit.Modifiers.Count(x => x.IsHidden));
            }

            this.log.Display(item);
        });
    }
}
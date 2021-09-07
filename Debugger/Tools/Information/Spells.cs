namespace Debugger.Tools.Information;

using System.Linq;
using System.Text;

using Debugger.Menus;

using Divine.Entity;
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

internal class Spells : IDebuggerTool
{
    private readonly Player player;

    private MenuSwitcher autoUpdate;

    private MenuSwitcher information;

    private uint lastUnitInfo;

    private readonly ILog log;

    private IMainMenu mainMenu;

    private Menu menu;

    private MenuSwitcher showBehavior;

    private MenuSwitcher showCastRange;

    private MenuSwitcher showHidden;

    private MenuSwitcher showLevel;

    private MenuSwitcher showManaCost;

    private MenuSwitcher showSpecialData;

    private MenuSwitcher showTalents;

    private MenuSwitcher showTargetType;

    public Spells(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
        this.player = EntityManager.LocalPlayer;
    }

    public int LoadPriority { get; } = 81;

    public void Activate()
    {
        this.menu = this.mainMenu.InformationMenu.CreateMenu("Spells");

        this.information = this.menu.CreateSwitcher("Get", false);
        this.information.ValueChanged += this.InformationOnPropertyChanged;

        this.autoUpdate = this.menu.CreateSwitcher("Auto update", false);
        this.autoUpdate.ValueChanged += this.AutoUpdateOnPropertyChanged;

        this.showHidden = this.menu.CreateSwitcher("Show hidden", false);
        this.showTalents = this.menu.CreateSwitcher("Show talents", false);
        this.showLevel = this.menu.CreateSwitcher("Show levels", false);
        this.showManaCost = this.menu.CreateSwitcher("Show mana cost", false);
        this.showCastRange = this.menu.CreateSwitcher("Show cast range", false);
        this.showBehavior = this.menu.CreateSwitcher("Show behavior", false);
        this.showTargetType = this.menu.CreateSwitcher("Show target type", false);
        this.showSpecialData = this.menu.CreateSwitcher("Show all special data", false);

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

            var item = new LogItem(LogType.Spell, Color.PaleGreen, "Spells information");

            item.AddLine("Unit name: " + unit.Name, unit.Name);
            item.AddLine("Unit network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("Unit classID: " + unit.ClassId, unit.ClassId);

            var localizeName = LocalizationHelper.LocalizeName(unit);
            item.AddLine("Unit display name: " + localizeName, localizeName);

            foreach (var ability in unit.Spellbook.Spells.Reverse())
            {
                if (!this.showHidden && ability.IsHidden)
                {
                    continue;
                }

                if (!this.showTalents && ability.Name.StartsWith("special_"))
                {
                    continue;
                }

                var abilityItem = new LogItem(LogType.Spell, Color.PaleGreen);

                abilityItem.AddLine("Name: " + ability.Name, ability.Name);
                abilityItem.AddLine("Network name: " + ability.NetworkName, ability.NetworkName);
                abilityItem.AddLine("ClassID: " + ability.ClassId, ability.ClassId);

                var localizeAbilityName = LocalizationHelper.LocalizeAbilityName(ability.Name);
                abilityItem.AddLine("Display name: " + localizeAbilityName, localizeAbilityName);

                if (this.showLevel)
                {
                    abilityItem.AddLine("Level: " + ability.Level, ability.Level);
                }

                if (this.showManaCost)
                {
                    abilityItem.AddLine("Mana cost: " + ability.ManaCost, ability.ManaCost);
                }

                if (this.showCastRange)
                {
                    abilityItem.AddLine("Cast range: " + ability.CastRange, ability.CastRange);
                }

                if (this.showBehavior)
                {
                    abilityItem.AddLine("Behavior: " + ability.AbilityBehavior, ability.AbilityBehavior);
                }

                if (this.showTargetType)
                {
                    abilityItem.AddLine("Target type: " + ability.TargetType, ability.TargetType);
                    abilityItem.AddLine("Target team type: " + ability.TargetTeamType, ability.TargetTeamType);
                }

                if (this.showSpecialData)
                {
                    abilityItem.AddLine("Special data =>");
                    foreach (var abilitySpecialData in ability.AbilitySpecialData.Where(x => !x.Name.StartsWith("#")))
                    {
                        var values = new StringBuilder();
                        var count = abilitySpecialData.Count;

                        for (uint i = 0; i < count; i++)
                        {
                            values.Append(abilitySpecialData.GetValue(i));
                            if (i < count - 1)
                            {
                                values.Append(", ");
                            }
                        }

                        abilityItem.AddLine("  " + abilitySpecialData.Name + ": " + values, abilitySpecialData.Name);
                    }
                }

                this.log.Display(abilityItem);
            }

            this.log.Display(item);
        });
    }
}
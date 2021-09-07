namespace Debugger.Tools.OnAddRemove;

using System.Collections.Generic;

using Debugger.Menus;

using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Update;

using Logger;

internal class Units : IDebuggerTool
{
    private readonly HashSet<string> ignored = new()
    {
        "portrait_world_unit"
    };

    private MenuSwitcher addEnabled;

    private MenuSwitcher heroesOnly;

    private MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private IMainMenu mainMenu;

    private Menu menu;

    private MenuSwitcher removeEnabled;

    public Units(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
    }

    public int LoadPriority { get; } = 100;

    public void Activate()
    {
        this.menu = this.mainMenu.OnAddRemoveMenu.CreateMenu("Units");

        this.addEnabled = this.menu.CreateSwitcher("On add enabled", false);
        this.addEnabled.SetTooltip("EntityManager<Unit>.EntityAdded");
        this.addEnabled.ValueChanged += this.AddEnabledPropertyChanged;

        this.removeEnabled = this.menu.CreateSwitcher("On remove enabled", false);
        this.removeEnabled.SetTooltip("EntityManager<Unit>.EntityRemoved");
        this.removeEnabled.ValueChanged += this.RemoveEnabledOnPropertyChanged;

        this.heroesOnly = this.menu.CreateSwitcher("Heroes only", false);
        this.ignoreUseless = this.menu.CreateSwitcher("Ignore useless", true);

        this.AddEnabledPropertyChanged(null, null);
        this.RemoveEnabledOnPropertyChanged(null, null);
    }

    public void Dispose()
    {
        this.addEnabled.ValueChanged -= this.AddEnabledPropertyChanged;
        this.removeEnabled.ValueChanged -= this.RemoveEnabledOnPropertyChanged;
        EntityManager.EntityAdded -= this.EntityManagerOnEntityAdded;
        EntityManager.EntityRemoved -= this.EntityManagerOnEntityRemoved;
    }

    private void AddEnabledPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.addEnabled)
            {
                this.menu.AddAsterisk();
                EntityManager.EntityAdded += this.EntityManagerOnEntityAdded;
            }
            else
            {
                if (!this.removeEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                EntityManager.EntityAdded -= this.EntityManagerOnEntityAdded;
            }
        });
    }

    private void EntityManagerOnEntityAdded(EntityAddedEventArgs e)
    {
        if (e.IsCollection)
        {
            return;
        }

        if (e.Entity is not Unit unit)
        {
            return;
        }

        if (!this.IsValid(unit))
        {
            return;
        }

        var item = new LogItem(LogType.Unit, Color.LightGreen, "Unit added");

        item.AddLine("Name: " + unit.Name, unit.Name);
        item.AddLine("Network name: " + unit.NetworkName, unit.NetworkName);
        item.AddLine("ClassID: " + unit.ClassId, unit.ClassId);
        item.AddLine("Position: " + unit.Position, unit.Position);
        item.AddLine("Attack capability: " + unit.AttackCapability, unit.AttackCapability);
        item.AddLine("Move capability: " + unit.MoveCapability, unit.MoveCapability);
        item.AddLine("Vision: " + unit.DayVision + "/" + unit.NightVision, unit.DayVision + "/" + unit.NightVision);
        item.AddLine("Health: " + unit.Health, unit.Health);

        this.log.Display(item);
    }

    private void EntityManagerOnEntityRemoved(EntityRemovedEventArgs e)
    {
        if (e.Entity is not Unit unit)
        {
            return;
        }

        if (!this.IsValid(unit))
        {
            return;
        }

        var item = new LogItem(LogType.Unit, Color.LightPink, "Unit removed");

        item.AddLine("Name: " + unit.Name, unit.Name);
        item.AddLine("Network name: " + unit.NetworkName, unit.NetworkName);
        item.AddLine("ClassID: " + unit.ClassId, unit.ClassId);
        item.AddLine("Position: " + unit.Position, unit.Position);

        this.log.Display(item);
    }

    private bool IsValid(Entity entity)
    {
        if (entity?.IsValid != true)
        {
            return false;
        }

        if (this.ignoreUseless && this.ignored.Contains(entity.Name))
        {
            return false;
        }

        if (this.heroesOnly && !(entity is Hero))
        {
            return false;
        }

        return true;
    }

    private void RemoveEnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.removeEnabled)
            {
                this.menu.AddAsterisk();
                EntityManager.EntityRemoved += this.EntityManagerOnEntityRemoved;
            }
            else
            {
                if (!this.addEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                EntityManager.EntityRemoved -= this.EntityManagerOnEntityRemoved;
            }
        });
    }
}
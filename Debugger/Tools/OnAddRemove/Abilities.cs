namespace Debugger.Tools.OnAddRemove;

using System.Collections.Generic;

using Debugger.Menus;

using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Update;

using Logger;

internal class Abilities : IDebuggerTool
{
    private readonly HashSet<string> ignored = new HashSet<string>
    {
        "special_",
        "_empty",
        "_hidden"
    };

    private MenuSwitcher addEnabled;

    private MenuSwitcher heroesOnly;

    private MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private IMainMenu mainMenu;

    private Menu menu;

    private MenuSwitcher removeEnabled;

    public Abilities(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
    }

    public int LoadPriority { get; } = 97;

    public void Activate()
    {
        this.menu = this.mainMenu.OnAddRemoveMenu.CreateMenu("Abilities");

        this.addEnabled = this.menu.CreateSwitcher("On add enabled", false);
        this.addEnabled.SetTooltip("EntityManager<Ability>.EntityAdded");
        this.addEnabled.ValueChanged += this.AddEnabledPropertyChanged;

        this.removeEnabled = this.menu.CreateSwitcher("On remove enabled", false);
        this.removeEnabled.SetTooltip("EntityManager<Ability>.EntityRemoved");
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

        if (e.Entity is not Ability ability)
        {
            return;
        }

        if (!this.IsValid(ability))
        {
            return;
        }

        var item = new LogItem(LogType.Ability, Color.LightGreen, "Ability added");

        item.AddLine("Name: " + ability.Name, ability.Name);
        item.AddLine("Network name: " + ability.NetworkName, ability.NetworkName);
        item.AddLine("ClassID: " + ability.ClassId, ability.ClassId);
        item.AddLine("Owner name: " + ability.Owner?.Name, ability.Owner?.Name);
        item.AddLine("Owner network name: " + ability.Owner?.NetworkName, ability.Owner?.NetworkName);
        item.AddLine("Owner classID: " + ability.Owner?.ClassId, ability.Owner?.ClassId);

        this.log.Display(item);
    }

    private void EntityManagerOnEntityRemoved(EntityRemovedEventArgs e)
    {
        if (e.Entity is not Ability ability)
        {
            return;
        }

        if (!this.IsValid(ability))
        {
            return;
        }

        var item = new LogItem(LogType.Ability, Color.LightPink, "Ability removed");

        item.AddLine("Name: " + ability.Name, ability.Name);
        item.AddLine("Network name: " + ability.NetworkName, ability.NetworkName);
        item.AddLine("ClassID: " + ability.ClassId, ability.ClassId);
        item.AddLine("Owner name: " + ability.Owner?.Name, ability.Owner?.Name);
        item.AddLine("Owner network name: " + ability.Owner?.NetworkName, ability.Owner?.NetworkName);
        item.AddLine("Owner classID: " + ability.Owner?.ClassId, ability.Owner?.ClassId);

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

        if (this.heroesOnly && !(entity.Owner is Hero))
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
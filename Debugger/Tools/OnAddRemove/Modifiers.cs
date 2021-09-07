namespace Debugger.Tools.OnAddRemove;

using System.Collections.Generic;
using System.Threading.Tasks;

using Debugger.Menus;

using Divine.Entity.Entities.Units.Heroes;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.Modifier.Modifiers;
using Divine.Numerics;
using Divine.Update;

using Logger;

internal class Modifiers : IDebuggerTool
{
    private readonly HashSet<string> ignored = new HashSet<string>
    {
        "modifier_projectile_vision",
        "modifier_truesight",
        "modifier_creep_haste",
        "modifier_creep_slow",
        "modifier_tower_aura",
        "modifier_tower_truesight_aura"
    };

    private MenuSwitcher addEnabled;

    private MenuSwitcher heroesOnly;

    private MenuSwitcher ignoreUseless;

    private readonly ILog log;

    private IMainMenu mainMenu;

    private Menu menu;

    private MenuSwitcher removeEnabled;

    public Modifiers(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
    }

    public int LoadPriority { get; } = 99;

    public void Activate()
    {
        this.menu = this.mainMenu.OnAddRemoveMenu.CreateMenu("Modifiers");

        this.addEnabled = this.menu.CreateSwitcher("On add enabled", false);
        this.addEnabled.SetTooltip("Unit.OnModifierAdded");
        this.addEnabled.ValueChanged += this.AddEnabledPropertyChanged;

        this.removeEnabled = this.menu.CreateSwitcher("On remove enabled", false);
        this.removeEnabled.SetTooltip("Unit.OnModifierRemoved");
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
        ModifierManager.ModifierAdded -= this.UnitOnModifierAdded;
        ModifierManager.ModifierRemoved -= this.UnitOnModifierRemoved;
    }

    private void AddEnabledPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.addEnabled)
            {
                this.menu.AddAsterisk();
                ModifierManager.ModifierAdded += this.UnitOnModifierAdded;
            }
            else
            {
                if (!this.removeEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                ModifierManager.ModifierAdded -= this.UnitOnModifierAdded;
            }
        });
    }

    private bool IsValid(Modifier modifier)
    {
        if (modifier?.IsValid != true)
        {
            return false;
        }

        var sender = modifier.Owner;
        if (sender?.IsValid != true)
        {
            return false;
        }

        if (this.heroesOnly && !(sender is Hero))
        {
            return false;
        }

        if (this.ignoreUseless && this.ignored.Contains(modifier.Name))
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
                ModifierManager.ModifierRemoved += this.UnitOnModifierRemoved;
            }
            else
            {
                if (!this.addEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                ModifierManager.ModifierRemoved -= this.UnitOnModifierRemoved;
            }
        });
    }

    private async void UnitOnModifierAdded(ModifierAddedEventArgs e)
    {
        if (e.IsCollection)
        {
            return;
        }

        await Task.Delay(1);
        var modifier = e.Modifier;

        if (!this.IsValid(modifier))
        {
            return;
        }

        var item = new LogItem(LogType.Modifier, Color.LightGreen, "Modifier added");

        item.AddLine("Name: " + modifier.Name, modifier.Name);
        item.AddLine("Texture name: " + modifier.TextureName, modifier.TextureName);
        item.AddLine("Elapsed time: " + modifier.ElapsedTime, modifier.ElapsedTime);
        item.AddLine("Remaining time: " + modifier.RemainingTime, modifier.RemainingTime);

        var sender = modifier.Owner;
        item.AddLine("Sender name: " + sender.Name, sender.Name);
        item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
        item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);

        this.log.Display(item);
    }

    private void UnitOnModifierRemoved(ModifierRemovedEventArgs e)
    {
        var modifier = e.Modifier;

        if (!this.IsValid(modifier))
        {
            return;
        }

        var item = new LogItem(LogType.Modifier, Color.LightPink, "Modifier removed");

        item.AddLine("Name: " + modifier.Name, modifier.Name);
        item.AddLine("Texture name: " + modifier.TextureName, modifier.TextureName);

        var sender = modifier.Owner;
        item.AddLine("Sender name: " + sender.Name, sender.Name);
        item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
        item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);

        this.log.Display(item);
    }
}
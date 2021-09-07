namespace Debugger.Tools.OnChange;

using Debugger.Menus;

using Divine.Entity.Entities;
using Divine.Entity.Entities.EventArgs;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Update;

using Logger;

internal class Animations : IDebuggerTool
{
    private MenuSwitcher enabled;

    private MenuSwitcher heroesOnly;

    private readonly ILog log;

    private IMainMenu mainMenu;

    private Menu menu;

    public Animations(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
    }

    public int LoadPriority { get; } = 95;

    public void Activate()
    {
        this.menu = this.mainMenu.OnChangeMenu.CreateMenu("Animations");

        this.enabled = this.menu.CreateSwitcher("Enabled", false);
        this.enabled.SetTooltip("Entity.OnAnimationChanged");
        this.enabled.ValueChanged += this.EnabledOnPropertyChanged;

        this.heroesOnly = this.menu.CreateSwitcher("Heroes only", false);

        this.EnabledOnPropertyChanged(null, null);
    }

    public void Dispose()
    {
        this.enabled.ValueChanged -= this.EnabledOnPropertyChanged;
        Entity.AnimationChanged -= this.EntityOnAnimationChanged;
    }

    private void EnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Entity.AnimationChanged += this.EntityOnAnimationChanged;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Entity.AnimationChanged -= this.EntityOnAnimationChanged;
            }
        });
    }

    private void EntityOnAnimationChanged(Entity sender, AnimationChangedEventArgs e)
    {
        if (!this.IsValid(sender))
        {
            return;
        }

        var item = new LogItem(LogType.Animation, Color.Cyan, "Animation changed");

        item.AddLine("Name: " + e.Name, e.Name);
        item.AddLine("Sender name: " + sender.Name, sender.Name);
        item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
        item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);

        this.log.Display(item);
    }

    private bool IsValid(Entity sender)
    {
        if (this.heroesOnly && !(sender is Hero))
        {
            return false;
        }

        return true;
    }
}
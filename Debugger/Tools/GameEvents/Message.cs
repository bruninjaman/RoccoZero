namespace Debugger.Tools.GameEvents;


using Debugger.Menus;

using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

using Logger;

internal class Message : IDebuggerTool
{
    private MenuSwitcher enabled;

    private IMainMenu mainMenu;

    private readonly ILog log;

    private Menu menu;

    public Message(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
    }

    public int LoadPriority { get; } = 82;

    public void Activate()
    {
        this.menu = this.mainMenu.GameEventsMenu.CreateMenu("Message");

        this.enabled = this.menu.CreateSwitcher("Enabled", false);
        this.enabled.SetTooltip("Game.OnMessage");
        this.enabled.ValueChanged += this.EnabledOnPropertyChanged;

        this.EnabledOnPropertyChanged(null, null);
    }

    public void Dispose()
    {
        this.enabled.ValueChanged -= this.EnabledOnPropertyChanged;
        //GameManager.OnMessage -= this.OnMessage;
    }

    private void EnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                //GameManager.OnMessage += this.OnMessage;
            }
            else
            {
                this.menu.RemoveAsterisk();
                // GameManager.OnMessage -= this.OnMessage;
            }
        });
    }

    /*private void OnMessage(MessageEventArgs args)
    {
        var item = new LogItem(LogType.GameEvent, Color.Yellow, "Message");

        item.AddLine("Name: " + args.Message, args.Message);

        this.log.Display(item);
    }*/
}
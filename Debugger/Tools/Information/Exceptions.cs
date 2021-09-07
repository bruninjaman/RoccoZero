namespace Debugger.Tools.Information;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Debugger.Menus;

using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Update;

using Logger;

internal class Exceptions : IDebuggerTool
{
    private TextWriter defaultOutput;

    private MenuSwitcher enabled;

    private readonly ILog log;

    private IMainMenu mainMenu;

    private Menu menu;

    private StringWriter output;

    private UpdateHandler updateHandler;

    public Exceptions(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
    }

    public int LoadPriority { get; } = 1;

    public void Activate()
    {
        this.menu = this.mainMenu.InformationMenu.CreateMenu("Exceptions");

        this.enabled = this.menu.CreateSwitcher("Enabled", false);
        this.enabled.ValueChanged += this.EnabledOnPropertyChanged;

        this.defaultOutput = Console.Out;

        this.updateHandler = UpdateManager.CreateIngameUpdate(200, false, this.Check);
        this.EnabledOnPropertyChanged(null, null);
    }

    public void Dispose()
    {
        this.enabled.ValueChanged -= this.EnabledOnPropertyChanged;
        UpdateManager.CreateIngameUpdate(this.Check);
        Console.SetOut(this.defaultOutput);
        this.output?.Dispose();
    }

    private void Check()
    {
        var text = this.output.ToString();
        if (!text.Any())
        {
            return;
        }

        try
        {
            var match = Regex.Match(text, @"(.*?exception)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var item = new LogItem(LogType.Exception, Color.Red, "Exception");
                item.AddLine(match.Value.Split(' ').Last());
                this.log.Display(item);

                Console.ForegroundColor = ConsoleColor.Red;
            }
        }
        finally
        {
            Console.SetOut(this.defaultOutput);
            Console.Write(text);
            Console.ResetColor();
            this.output.Dispose();
            Console.SetOut(this.output = new StringWriter());
        }
    }

    private void EnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                this.updateHandler.IsEnabled = true;
                this.output?.Dispose();
                Console.SetOut(this.output = new StringWriter());
            }
            else
            {
                this.menu.RemoveAsterisk();
                this.updateHandler.IsEnabled = false;
                Console.SetOut(this.defaultOutput);
            }
        });
    }
}
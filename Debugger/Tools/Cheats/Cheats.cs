namespace Debugger.Tools.Cheats;

using Divine.Input;

using Debugger.Menus;

using Divine.GameConsole;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

internal class Cheats : IDebuggerTool
{
    private bool allVisionEnabled;

    private MenuHoldKey bot25Lvl;

    private MenuHoldKey creeps;

    private bool creepsEnabled;

    private MenuHoldKey hero25Lvl;

    private MenuHoldKey heroGold;

    private readonly IMainMenu mainMenu;

    private Menu menu;

    private MenuHoldKey refresh;

    private MenuHoldKey vision;

    private MenuHoldKey wtf;

    private bool wtfEnabled;

    public Cheats(IMainMenu mainMenu)
    {
        this.mainMenu = mainMenu;
    }

    public int LoadPriority { get; } = 5;

    public void Activate()
    {
        this.menu = this.mainMenu.CheatsMenu;

        this.refresh = this.menu.CreateHoldKey("Refresh", Key.NumPad3);
        this.refresh.ValueChanged += this.RefreshOnPropertyChanged;

        this.wtf = this.menu.CreateHoldKey("Change wtf", Key.Divide);
        this.wtf.ValueChanged += this.WtfOnPropertyChanged;

        this.vision = this.menu.CreateHoldKey("Change vision", Key.Multiply);
        this.vision.ValueChanged += this.VisionOnPropertyChanged;
        this.allVisionEnabled = GameConsoleManager.GetInt32("dota_all_vision") == 1;

        this.creeps = this.menu.CreateHoldKey("Change creeps spawn", Key.NumPad0);
        this.creeps.ValueChanged += this.CreepsOnPropertyChanged;

        this.hero25Lvl = this.menu.CreateHoldKey("Hero 25lvl", Key.NumPad1);
        this.hero25Lvl.ValueChanged += this.Hero25LvlOnPropertyChanged;

        this.heroGold = this.menu.CreateHoldKey("Hero gold", Key.NumPad1);
        this.heroGold.ValueChanged += this.HeroGoldOnPropertyChanged;

        this.bot25Lvl = this.menu.CreateHoldKey("Bot 25lvl", Key.NumPad2);
        this.bot25Lvl.ValueChanged += this.Bot25LvlOnPropertyChanged;
    }

    public void Dispose()
    {
        this.refresh.ValueChanged -= this.RefreshOnPropertyChanged;
        this.wtf.ValueChanged -= this.WtfOnPropertyChanged;
        this.vision.ValueChanged -= this.VisionOnPropertyChanged;
        this.creeps.ValueChanged -= this.CreepsOnPropertyChanged;
        this.hero25Lvl.ValueChanged -= this.Hero25LvlOnPropertyChanged;
        this.heroGold.ValueChanged -= this.HeroGoldOnPropertyChanged;
        this.bot25Lvl.ValueChanged -= this.Bot25LvlOnPropertyChanged;
    }

    private void Bot25LvlOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.bot25Lvl)
            {
                GameConsoleManager.ExecuteCommand("dota_bot_give_level 25");
            }
        });
    }

    private void CreepsOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.creeps)
            {
                if (this.creepsEnabled)
                {
                    GameConsoleManager.ExecuteCommand("dota_creeps_no_spawning_disable");
                    this.creepsEnabled = false;
                }
                else
                {
                    GameConsoleManager.ExecuteCommand("dota_creeps_no_spawning_enable");
                    this.creepsEnabled = true;
                }
            }
        });
    }

    private void Hero25LvlOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.hero25Lvl)
            {
                GameConsoleManager.ExecuteCommand("dota_hero_level 25");
            }
        });
    }

    private void HeroGoldOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.heroGold)
            {
                GameConsoleManager.ExecuteCommand("dota_give_gold 99999");
            }
        });
    }

    private void RefreshOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.refresh)
            {
                GameConsoleManager.ExecuteCommand("dota_hero_refresh");
            }
        });
    }

    private void VisionOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.vision)
            {
                if (this.allVisionEnabled)
                {
                    GameConsoleManager.ExecuteCommand("dota_all_vision_disable");
                    this.allVisionEnabled = false;
                }
                else
                {
                    GameConsoleManager.ExecuteCommand("dota_all_vision_enable");
                    this.allVisionEnabled = true;
                }
            }
        });
    }

    private void WtfOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.wtf)
            {
                if (this.wtfEnabled)
                {
                    GameConsoleManager.ExecuteCommand("dota_ability_debug_disable");
                    this.wtfEnabled = false;
                }
                else
                {
                    GameConsoleManager.ExecuteCommand("dota_ability_debug_enable");
                    this.wtfEnabled = true;
                }
            }
        });
    }
}
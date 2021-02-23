namespace Debugger.Tools.GameEvents
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Debugger.Menus;

    using Divine;
    using Divine.Menu.EventArgs;
    using Divine.Menu.Items;

    using Logger;

    using SharpDX;

    internal class FireEvent : IDebuggerTool
    {
        private readonly HashSet<string> ignored = new HashSet<string>
        {
            "dota_action_success"
        };

        private MenuSwitcher enabled;

        private MenuSwitcher ignoreUseless;

        private IMainMenu mainMenu;

        private readonly ILog log;

        private Menu menu;

        public FireEvent(IMainMenu mainMenu, ILog log)
        {
            this.mainMenu = mainMenu;
            this.log = log;
        }

        public int LoadPriority { get; } = 85;

        public void Activate()
        {
            this.menu = this.mainMenu.GameEventsMenu.CreateMenu("Fire event");

            this.enabled = this.menu.CreateSwitcher("Enabled", false);
            this.enabled.SetTooltip("Game.OnFireEvent");
            this.enabled.ValueChanged += this.EnabledOnPropertyChanged;

            this.ignoreUseless = this.menu.CreateSwitcher("Ignore useless", true);

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.ValueChanged -= this.EnabledOnPropertyChanged;
            GameManager.FireEvent -= this.GameOnFireEvent;
        }

        private void EnabledOnPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (this.enabled)
                {
                    this.menu.AddAsterisk();
                    GameManager.FireEvent += this.GameOnFireEvent;
                }
                else
                {
                    this.menu.RemoveAsterisk();
                    GameManager.FireEvent -= this.GameOnFireEvent;
                }
            });
        }

        private void GameOnFireEvent(FireEventEventArgs e)
        {
            if (!this.IsValid(e))
            {
                return;
            }

            var item = new LogItem(LogType.GameEvent, Color.Yellow, "Fire event");

            item.AddLine("Name: " + e.Name, e.Name);

            this.log.Display(item);
        }

        private bool IsValid(FireEventEventArgs e)
        {
            if (this.ignoreUseless && this.ignored.Contains(e.Name))
            {
                return false;
            }

            return true;
        }
    }
}
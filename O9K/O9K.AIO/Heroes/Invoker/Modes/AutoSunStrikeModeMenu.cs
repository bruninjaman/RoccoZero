using O9K.AIO.Modes.Permanent;
using O9K.Core.Managers.Menu.Items;

namespace O9K.AIO.Heroes.Invoker.Modes
{
    internal class AutoSunStrikeModeMenu : PermanentModeMenu
    {
        public MenuSwitcher killStealOnly { get; set; }

        public AutoSunStrikeModeMenu(Core.Managers.Menu.Items.Menu rootMenu, string displayName, string tooltip = null)
            : base(rootMenu, displayName, tooltip)
        {
            killStealOnly = Menu.Add(new MenuSwitcher("Kill steal only", true));
            // onStunnedOnly = Menu.Add(new MenuSwitcher("Only for stunned enemies", true));
            // canInvoke = Menu.Add(new MenuSwitcher("Invoke if needed", true));
            moveCamera = Menu.Add(new MenuSwitcher("Move camera", true));
            delay = Menu.Add(new MenuSlider("Delay", 0, 0, 2000));
        }

        public MenuSlider delay { get; set; }

        public MenuSwitcher moveCamera { get; set; }

        // public MenuSwitcher canInvoke { get; set; }

        // public MenuSwitcher onStunnedOnly { get; set; }
    }
}
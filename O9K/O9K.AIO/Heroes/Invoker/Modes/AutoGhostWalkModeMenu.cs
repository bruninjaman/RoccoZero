using O9K.AIO.Modes.Permanent;
using O9K.Core.Managers.Menu.Items;

namespace O9K.AIO.Heroes.Invoker.Modes
{
    internal class AutoGhostWalkModeMenu : PermanentModeMenu
    {
        public AutoGhostWalkModeMenu(Core.Managers.Menu.Items.Menu rootMenu, string displayName, string tooltip = null)
            : base(rootMenu, displayName, tooltip)
        {
            hpSlider = Menu.Add(new MenuSlider("Hp for cast", 20, 5, 90));
        }

        public MenuSlider hpSlider { get; }
    }
}
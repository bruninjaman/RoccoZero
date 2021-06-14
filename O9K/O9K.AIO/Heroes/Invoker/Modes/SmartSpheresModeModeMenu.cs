using O9K.AIO.Modes.Permanent;
using O9K.Core.Managers.Menu;
using O9K.Core.Managers.Menu.Items;

namespace O9K.AIO.Heroes.Invoker.Modes
{
    internal class SmartSpheresModeModeMenu : PermanentModeMenu
    {
        public SmartSpheresModeModeMenu(Core.Managers.Menu.Items.Menu rootMenu, string displayName, string tooltip = null)
            : base(rootMenu, displayName, tooltip)
        {
            hpSlider = Menu.Add(new MenuSlider("Health for quas", 75, 0, 100));
            hpSlider.SetTooltip("Change on move to quas when hp% is lower");
            hpSlider.AddTranslation(Lang.Ru, "Жизни для quas");
            hpSlider.AddTooltipTranslation(Lang.Ru, "Использовать quas, а не wex при движении, когда процент хп меньше выбранного значения");
            // this.hpSlider.AddTranslation(Lang.Cn, "剩余魔法值");
            // this.hpSlider.AddTooltipTranslation(Lang.Cn, "显示其余或所需的魔法值");
        }

        public MenuSlider hpSlider { get; }
    }
}
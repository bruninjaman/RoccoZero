using Divine.Core.ComboFactory.Menus.AutoItems;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.ComboFactory.Menus.Settings;
using Divine.Core.ComboFactory.Menus.TargetSelector;
using Divine.Core.Managers.Menu;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus
{
    public abstract class BaseMenuConfig
    {
        protected BaseMenuConfig()
        {
            if (MoreMenu == null)
            {
                MoreMenu = new BaseMoreMenu();
            }

            if (FarmMenu == null)
            {
                FarmMenu = new BaseFarmMenu();
            }

            if (DisableMenu == null)
            {
                DisableMenu = new BaseDisableMenu();
            }

            if (BladeMailMenu == null)
            {
                BladeMailMenu = new BaseBladeMailMenu();
            }

            if (RadiusMenu == null)
            {
                RadiusMenu = new BaseRadiusMenu();
            }

            MenuFactory.RegisterMenu(this);
        }

        public void Dispose()
        {
            MenuFactory.DeregisterMenu(this);
        }

        [Menu("Combo")]
        [Order(1)]
        public abstract BaseComboMenu ComboMenu { get; }

        [Menu("Kill Steal")]
        [Order(2)]
        public abstract BaseKillStealMenu KillStealMenu { get; }

        [Menu("More")]
        [Order(3)]
        public virtual BaseMoreMenu MoreMenu { get; }

        [Menu("Farm")]
        [Order(4)]
        public virtual BaseFarmMenu FarmMenu { get; }

        [Menu("Escape")]
        [Order(5)]
        public BaseEscapeMenu EscapeMenu { get; } = new BaseEscapeMenu();

        [Menu("Disable"), TextureDivine(@"items\item_sheepstick.png")]
        [Order(7)]
        public virtual BaseDisableMenu DisableMenu { get; } = new BaseDisableMenu();

        [Menu("Linken Breaker"), TextureDivine(@"items\item_sphere.png")]
        [Order(8)]
        public abstract BaseLinkenBreakerMenu LinkenBreakerMenu { get; }

        [Menu("Auto Items")]
        [Order(9)]
        public AutoItemsMenu AutoItemsMenu { get; } = new AutoItemsMenu();

        [Menu("Blade Mail"), TextureDivine(@"items\item_blade_mail.png")]
        [Order(10)]
        public virtual BaseBladeMailMenu BladeMailMenu { get; }

        [Menu("Radius")]
        [Order(11)]
        public virtual BaseRadiusMenu RadiusMenu { get; }

        [Menu("Target Selector")]
        [Order(12)]
        public BaseTargetSelectorMenu TargetSelectorMenu { get; } = new BaseTargetSelectorMenu();

        [Menu("Settings")]
        [Order(13)]
        public BaseSettingsMenu SettingsMenu { get; } = new BaseSettingsMenu();
    }
}

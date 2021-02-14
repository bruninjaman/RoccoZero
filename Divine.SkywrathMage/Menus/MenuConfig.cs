using Divine.Menu;
using Divine.Menu.Items;
using Divine.SkywrathMage.Menus.Combo;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class MenuConfig
    {
        private readonly RootMenu RootMenu;

        public MenuConfig()
        {
            RootMenu = MenuManager.CreateRootMenu("Divine.SkywrathMage").SetHeroTexture(HeroId.npc_dota_hero_skywrath_mage);
            ComboMenu = new ComboMenu(RootMenu);
            KillStealMenu = new AutoKillStealMenu(RootMenu);
            MoreMenu = new MoreMenu(RootMenu);
            FarmMenu = new FarmMenu(RootMenu);
            DisableMenu = new DisableMenu(RootMenu);
            LinkenBreakerMenu = new LinkenBreakerMenu(RootMenu);
            BladeMailMenu = new BladeMailMenu(RootMenu);
            RadiusMenu = new RadiusMenu(RootMenu);
            TargetSelectorMenu = new TargetSelectorMenu(RootMenu);
            SettingsMenu = new SettingsMenu(RootMenu);
        }

        public ComboMenu ComboMenu { get; }

        public AutoKillStealMenu KillStealMenu { get; }

        public MoreMenu MoreMenu { get; }

        public FarmMenu FarmMenu { get; }

        public DisableMenu DisableMenu { get; }

        public LinkenBreakerMenu LinkenBreakerMenu { get; }

        public BladeMailMenu BladeMailMenu { get; }

        public RadiusMenu RadiusMenu { get; }

        public TargetSelectorMenu TargetSelectorMenu { get; }

        public SettingsMenu SettingsMenu { get; }
    }
}
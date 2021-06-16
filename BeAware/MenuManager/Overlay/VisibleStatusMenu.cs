using Divine.Menu.Items;

namespace BeAware.MenuManager.Overlay
{
    internal sealed class VisibleStatusMenu
    {
        public VisibleStatusMenu(Menu topPanelMenu)
        {
            var visibleStatusMenu = topPanelMenu.CreateMenu("Visible Status");
            VisibleStatusAllyItem = visibleStatusMenu.CreateSwitcher("Ally", false);
            VisibleStatusEnemyItem = visibleStatusMenu.CreateSwitcher("Enemy", false);
            visibleStatusMenu.CreateText("");
            EnemyNotVisibleTimeItem = visibleStatusMenu.CreateSwitcher("Enemy Not Visible Time", false);
            SizeItem = visibleStatusMenu.CreateSlider("Size:", 20, 5, 50);
            RedItem = visibleStatusMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = visibleStatusMenu.CreateSlider("Green:", 255, 0, 255);
            BlueItem = visibleStatusMenu.CreateSlider("Blue:", 0, 0, 255);
        }

        public MenuSwitcher VisibleStatusAllyItem { get; }

        public MenuSwitcher VisibleStatusEnemyItem { get; }

        public MenuSwitcher EnemyNotVisibleTimeItem { get; }

        public MenuSlider SizeItem { get; }

        public MenuSlider RedItem { get; }

        public MenuSlider GreenItem { get; }

        public MenuSlider BlueItem { get; }
    }
}
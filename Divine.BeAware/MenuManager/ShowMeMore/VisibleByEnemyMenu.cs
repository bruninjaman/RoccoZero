using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore
{
    internal sealed class VisibleByEnemyMenu
    {
        private readonly string[] EffectTypeNames =
        {
            "Default",
            "Default MOD",
            "VBE",
            "Omniknight",
            "Assault",
            "Arrow",
            "Mark",
            "Glyph",
            "Coin",
            "Lightning",
            "Energy Orb",
            "Pentagon",
            "Axis",
            "Beam Jagged",
            "Beam Rainbow",
            "Walnut Statue",
            "Thin Thick",
            "Ring Wave",
            "Visible"
        };

        public VisibleByEnemyMenu(Menu.Items.Menu showMeMoreMenu)
        {
            var visibleByEnemyMenu = showMeMoreMenu.CreateMenu("Visible By Enemy");
            EnableItem = visibleByEnemyMenu.CreateSwitcher("Enable");
            EffectTypeItem = visibleByEnemyMenu.CreateSelector("Effect Type", EffectTypeNames);
            RedItem = visibleByEnemyMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = visibleByEnemyMenu.CreateSlider("Green:", 255, 0, 255);
            BlueItem = visibleByEnemyMenu.CreateSlider("Blue:", 255, 0, 255);
            AlphaItem = visibleByEnemyMenu.CreateSlider("Alpha:", 255, 0, 255);
            AlliedHeroesItem = visibleByEnemyMenu.CreateSwitcher("Allied Heroes");
            WardsItem = visibleByEnemyMenu.CreateSwitcher("Wards");
            MinesItem = visibleByEnemyMenu.CreateSwitcher("Mines");
            OutpostsItem = visibleByEnemyMenu.CreateSwitcher("Outposts");
            NeutralsItem = visibleByEnemyMenu.CreateSwitcher("Neutrals");
            UnitsItem = visibleByEnemyMenu.CreateSwitcher("Units");
            BuildingsItem = visibleByEnemyMenu.CreateSwitcher("Buildings");
        }

        public MenuSwitcher EnableItem  { get; }

        public MenuSelector EffectTypeItem  { get; }

        public MenuSlider RedItem { get; }

        public MenuSlider GreenItem { get; }

        public MenuSlider BlueItem { get; }

        public MenuSlider AlphaItem { get; }

        public MenuSwitcher AlliedHeroesItem  { get; }

        public MenuSwitcher WardsItem  { get; }

        public MenuSwitcher MinesItem  { get; }

        public MenuSwitcher OutpostsItem { get; }

        public MenuSwitcher NeutralsItem  { get; }

        public MenuSwitcher UnitsItem  { get; }

        public MenuSwitcher BuildingsItem  { get; }
    }
}
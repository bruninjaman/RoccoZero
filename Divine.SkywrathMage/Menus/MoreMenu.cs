namespace Divine.SkywrathMage.Menus
{
    internal sealed class MoreMenu
    {
        public MoreMenu(Menu.Items.Menu menu)
        {
            var moreMenu = menu.CreateMenu("More");
            AutoComboMenu = new AutoComboMenu(moreMenu);
            SmartArcaneBoltMenu = new SmartArcaneBoltMenu(moreMenu);
            SmartConcussiveShotMenu = new SmartConcussiveShotMenu(moreMenu);
        }

        public AutoComboMenu AutoComboMenu { get; } 

        public SmartArcaneBoltMenu SmartArcaneBoltMenu { get; }

        public SmartConcussiveShotMenu SmartConcussiveShotMenu { get; }
    }
}
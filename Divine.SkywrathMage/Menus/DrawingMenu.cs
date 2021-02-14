namespace Divine.SkywrathMage.Menus
{
    internal sealed class DrawingMenu
    {
        public DrawingMenu(Menu.Items.Menu menu)
        {
            var drawingMenu = menu.CreateMenu("Drawing");
            TextPanelMenu = new TextPanelMenu(drawingMenu);
            DamageCalculationMenu = new DamageCalculationMenu(drawingMenu);
        }

        public TextPanelMenu TextPanelMenu { get; }

        public DamageCalculationMenu DamageCalculationMenu { get; }
    }
}
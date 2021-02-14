using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class DamageCalculationMenu
    {
        public DamageCalculationMenu(Menu.Items.Menu menu)
        {
            var damageCalculationMenu = menu.CreateMenu("Damage Calculation");
            HpBarItem = damageCalculationMenu.CreateSwitcher("Hp Bar");
            ValueItem = damageCalculationMenu.CreateSwitcher("Value", false);
        }

        public MenuSwitcher HpBarItem { get; set; }

        public MenuSwitcher ValueItem { get; set; }
    }
}
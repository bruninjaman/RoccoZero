using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class RadiusMenu
    {
        public RadiusMenu(Menu.Items.Menu menu)
        {
            var radiusMenu = menu.CreateMenu("Radius");
            ArcaneBoltItem = radiusMenu.CreateSwitcher("Arcane Bolt");
            ConcussiveShotItem = radiusMenu.CreateSwitcher("Concussive Shot");
            AncientSealItem = radiusMenu.CreateSwitcher("Ancient Seal");
            MysticFlareItem = radiusMenu.CreateSwitcher("Mystic Flare");
            TargetHitConcussiveShotItem = radiusMenu.CreateSwitcher("Target Hit Concussive Shot");
            BlinkDaggerItem = radiusMenu.CreateSwitcher("Blink Dagger", false);
        }

        public MenuSwitcher ArcaneBoltItem { get; }

        public MenuSwitcher ConcussiveShotItem { get; }

        public MenuSwitcher AncientSealItem { get; }

        public MenuSwitcher MysticFlareItem { get; }

        public MenuSwitcher TargetHitConcussiveShotItem { get; }

        public MenuSwitcher BlinkDaggerItem { get; }
    }
}
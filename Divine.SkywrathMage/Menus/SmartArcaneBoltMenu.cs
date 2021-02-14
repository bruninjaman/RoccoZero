using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class SmartArcaneBoltMenu
    {
        public SmartArcaneBoltMenu(Menu.Items.Menu menu)
        {
            var smartArcaneBoltMenu = menu.CreateMenu("Smart Arcane Bolt").SetAbilityTexture(AbilityId.skywrath_mage_arcane_bolt);
            ToggleHotkeyItem = smartArcaneBoltMenu.CreateToggleKey("Toggle Hotkey");
            OwnerMinHealthItem = smartArcaneBoltMenu.CreateSlider("Owner Min Health % To Auto Arcane Bolt:", 20, 0, 70);

            smartArcaneBoltMenu.CreateText("SmartArcaneBoltMenuText", "");

            SpamHotkeyItem = smartArcaneBoltMenu.CreateHoldKey("Spam Hotkey");
            SpamUnitsItem = smartArcaneBoltMenu.CreateSwitcher("Spam Units");
            OrbwalkerItem = smartArcaneBoltMenu.CreateSelector("Orbwalker", new[] { "Distance", "Default", "Free", "Only Attack", "No Move" });
            MinDisInOrbwalkItem = smartArcaneBoltMenu.CreateSlider("Min Distance In Orbwalk:", 600, 200, 600);
            FullFreeModeItem = smartArcaneBoltMenu.CreateSwitcher("Full Free Mode", false);
        }

        public MenuToggleKey ToggleHotkeyItem { get; }

        public MenuSlider OwnerMinHealthItem { get; }

        public MenuHoldKey SpamHotkeyItem { get; }

        public MenuSwitcher SpamUnitsItem { get; }

        public MenuSelector OrbwalkerItem { get; }

        public MenuSlider MinDisInOrbwalkItem { get; }

        public MenuSwitcher FullFreeModeItem { get; }
    }
}
using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class MysticFlareMenu
    {
        public MysticFlareMenu(Menu.Items.Menu menu)
        {
            var mysticFlareMenu = menu.CreateMenu("Mystic Flare").SetAbilityTexture(AbilityId.skywrath_mage_mystic_flare);
            MinHealthToUltItem = mysticFlareMenu.CreateSlider("Target Min Health % To Ult:", 0, 0, 70);

            mysticFlareMenu.CreateText("MysticFlareMenu1", "");

            BadUltItem = mysticFlareMenu.CreateSwitcher("Bad Ult", false).SetTooltip("It is not recommended to enable this. If you do not have these items (RodofAtos, Hex, Ethereal) then this function is activated");
            BadUltMovementSpeedItem = mysticFlareMenu.CreateSlider("Bad Ult Movement Speed:", 500, 240, 500).SetTooltip("If the enemy has less Movement Speed from this value, then immediately ULT");
        }

        public MenuSlider MinHealthToUltItem { get; }

        public MenuSwitcher BadUltItem { get; }

        public MenuSlider BadUltMovementSpeedItem { get; }
    }
}
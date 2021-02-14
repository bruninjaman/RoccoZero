using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class TargetEffectsMenu
    {
        public TargetEffectsMenu(Menu.Items.Menu menu)
        {
            var targetEffectsMenu = menu.CreateMenu("Target Effects");

            EnableItem = targetEffectsMenu.CreateSwitcher("Enable");
            RedItem = targetEffectsMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = targetEffectsMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = targetEffectsMenu.CreateSlider("Blue:", 0, 0, 255);

            targetEffectsMenu.CreateText("Empty", "");

            FreeEnableItem = targetEffectsMenu.CreateSwitcher("Free Enable");
            FreeRedItem = targetEffectsMenu.CreateSlider("Red:", 0, 0, 255);
            FreeGreenItem = targetEffectsMenu.CreateSlider("Green:", 255, 0, 255);
            FreeBlueItem = targetEffectsMenu.CreateSlider("Blue:", 255, 0, 255);
            EffectTypeItem = targetEffectsMenu.CreateSelector("Effect Type:", new[] { "Default", "Without Circle" });
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSlider RedItem { get; }

        public MenuSlider GreenItem { get; }

        public MenuSlider BlueItem { get; }

        public MenuSwitcher FreeEnableItem { get; }

        public MenuSlider FreeRedItem { get; }

        public MenuSlider FreeGreenItem { get; }

        public MenuSlider FreeBlueItem { get; }

        public MenuSelector EffectTypeItem { get; }
    }
}
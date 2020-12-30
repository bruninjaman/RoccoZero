using Divine.BeAware.MenuManager.Overlay;
using Divine.BeAware.MenuManager.PartialMapHack;
using Divine.BeAware.MenuManager.ShowMeMore;
using Divine.Menu.Items;

using SharpDX;

namespace Divine.BeAware.MenuManager
{
    internal sealed class MenuConfig
    {
        public MenuConfig()
        {
            var rootMenu = Menu.MenuManager.CreateRootMenu("Divine.BeAware").SetTexture(@"Divine.BeAware.Resources.Textures.divinebeaware.png").SetFontColor(Color.Aqua);

            OverlayMenu = new OverlayMenu(rootMenu);
            ShowMeMoreMenu = new ShowMeMoreMenu(rootMenu);
            PartialMapHackMenu = new PartialMapHackMenu(rootMenu);

            FullyDisableSoundsItem = rootMenu.CreateSwitcher("Fully Disable Sounds", false);
            VolumeItem = rootMenu.CreateSlider("Volume", 100, 0, 100);
            DefaultSoundItem = rootMenu.CreateSwitcher("Default Sound", false).SetTooltip("All Sounds Becomes Default");
            LanguageItem = rootMenu.CreateSelector("Language", new[] { "EN", "RU" });
        }

        public OverlayMenu OverlayMenu { get; }

        public ShowMeMoreMenu ShowMeMoreMenu { get; }

        public PartialMapHackMenu PartialMapHackMenu { get; } 

        public MenuSwitcher FullyDisableSoundsItem { get; }

        public MenuSlider VolumeItem { get; }

        public MenuSwitcher DefaultSoundItem { get; }

        public MenuSelector LanguageItem { get; }

        public void Dispose()
        {

        }
    }
}
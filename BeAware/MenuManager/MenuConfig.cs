namespace BeAware.MenuManager;

using BeAware.MenuManager.Overlay;
using BeAware.MenuManager.PartialMapHack;
using BeAware.MenuManager.ShowMeMore;

using Divine.Menu.Items;
using Divine.Numerics;

internal sealed class MenuConfig
{
    public MenuConfig()
    {
        var rootMenu = Divine.Menu.MenuManager.CreateRootMenu("BeAware").SetImage(@"BeAware.Resources.Textures.divinebeaware.png").SetFontColor(Color.Aqua);

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
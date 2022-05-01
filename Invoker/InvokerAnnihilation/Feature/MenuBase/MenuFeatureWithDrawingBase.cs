using Divine.Input;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Plugins.HumanizerOld;

namespace InvokerAnnihilation.Feature.MenuBase;

public class MenuFeatureWithDrawingBase : IMenuFeatureWithDrawing
{
    protected MenuFeatureWithDrawingBase(RootMenu rootMenu, string name, Vector3 sizeSettings = default)
    {
        Menu = rootMenu.CreateMenu(name);
        Enable = Menu.CreateSwitcher("Enable");
        PositionXItem = Menu.CreateSlider("Position X:", (int) (HUDInfo.ScreenSize.X - 800), 0,
            (int) (HUDInfo.ScreenSize.X + 500));
        PositionYItem = Menu.CreateSlider("Position Y:", (int) (HUDInfo.ScreenSize.Y - 240), 0,
            (int) (HUDInfo.ScreenSize.Y + 500));
        if (sizeSettings == default)
            SizeItem = Menu.CreateSlider("Size:", 0, -20, 150);
        else
            SizeItem = Menu.CreateSlider("Size:", (int) sizeSettings.X, (int) sizeSettings.Y, (int) sizeSettings.Z);
    }

    public Menu Menu { get; set; }

    public MenuSwitcher Enable { get; set; }

    public MenuSlider SizeItem { get; set; }
    public MenuSlider PositionYItem { get; set; }
    public MenuSlider PositionXItem { get; set; }
}
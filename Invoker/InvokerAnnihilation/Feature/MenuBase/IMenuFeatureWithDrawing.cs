using Divine.Menu.Items;

namespace InvokerAnnihilation.Feature.MenuBase;

public interface IMenuFeatureWithDrawing : IMenuFeature
{
    public MenuSlider SizeItem { get; set; }

    public MenuSlider PositionYItem { get; set; }

    public MenuSlider PositionXItem { get; set; }
}
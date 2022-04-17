using Divine.Menu.Items;

namespace InvokerAnnihilation.Feature.MenuBase;

public interface IMenuFeatureWithHoldKey
{
    public MenuHoldKey HoldKey { get; set; }
}
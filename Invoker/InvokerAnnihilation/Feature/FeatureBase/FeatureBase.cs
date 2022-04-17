using Divine.Menu.EventArgs;
using Divine.Menu.Items;

namespace InvokerAnnihilation.Feature.FeatureBase;

public abstract class FeatureBase<T> : IFeature<T>
{
    private readonly MenuSwitcher _enableToggle;

    protected FeatureBase(MenuSwitcher menuSwitcher)
    {
        _enableToggle = menuSwitcher;
        _enableToggle.ValueChanged += MenuSwitcherOnValueChanged;
    }

    public abstract void Enable();

    public abstract void Disable();
    public abstract T CurrentMenu { get; set; }

    public void Dispose()
    {
        _enableToggle.ValueChanged -= MenuSwitcherOnValueChanged;
    }

    private void MenuSwitcherOnValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        if (e.Value)
            Enable();
        else
            Disable();
    }
}
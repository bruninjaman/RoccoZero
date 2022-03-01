using Divine.Menu.EventArgs;
using Divine.Menu.Items;

namespace TinkerEW;

internal sealed class TinkerMain
{
    public readonly Context Context;
    public readonly Menu Menu;
    private Combo? TinkerCombo;

    public TinkerMain(Context context)
    {
        Context = context;
        Menu = Context.Menu;

        Menu.EnableScript.ValueChanged += EnableScript_ValueChanged;
    }

    private void EnableScript_ValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        if (e.Value)
            Menu.ComboHoldKey.ValueChanged += ComboHoldKey_ValueChanged;
        else
            Menu.ComboHoldKey.ValueChanged -= ComboHoldKey_ValueChanged;
    }

    private void ComboHoldKey_ValueChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
    {
        if (Menu.EnableScript.Value)
        {
            if (e.Value)
                TinkerCombo = new Combo(Menu);
            else
                TinkerCombo?.Dispose();
        }
    }

    internal void Dispose()
    {

    }
}
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

using TinkerEW.Modules;

namespace TinkerEW;

internal sealed class TinkerMain
{
    public readonly Context Context;
    public readonly Menu Menu;
    private Combo? TinkerCombo;
    private FailSwitch FailSwitch;

    public TinkerMain(Context context)
    {
        Context = context;
        Menu = Context.Menu;

        Menu.EnableScript.ValueChanged += EnableScript_ValueChanged;
    }

    private void EnableScript_ValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        if (e.Value)
        {
            FailSwitch = new FailSwitch(Menu);
            Menu.ComboHoldKey.ValueChanged += ComboHoldKey_ValueChanged;
        }
        else
        {
            FailSwitch?.Dispose();
            Menu.ComboHoldKey.ValueChanged -= ComboHoldKey_ValueChanged;
        }
    }

    private void ComboHoldKey_ValueChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
    {
        if (Menu.EnableScript.Value)
        {
            if (e.Value)
            {
                TinkerCombo = new Combo(Menu);
            }
            else
            {
                TinkerCombo?.Dispose();
            }
        }
    }

    internal void Dispose()
    {

    }
}
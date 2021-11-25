namespace O9K.AIO.Heroes.Brewmaster.Modes;

using Core.Managers.Menu.Items;

using O9K.AIO.Modes.Base;

internal sealed class KeyBindModeMenu : BaseModeMenu
{
    public KeyBindModeMenu(Menu rootMenu, string displayName, string tooltip = null)
        : base(rootMenu, displayName)
    {
        this.DispelMagicKey = new MenuHoldKey("Dispel magic", "dispel" + this.SimplifiedName, Divine.Input.Key.None, true);
        this.CycloneKey = new MenuHoldKey("Cyclone", "cyclone" + this.SimplifiedName, Divine.Input.Key.None, true);
        this.WindWalkKey = new MenuHoldKey("Wind walk", "windWalk" + this.SimplifiedName, Divine.Input.Key.None, true);
        this.HurlBoulder = new MenuHoldKey("Hurl boulder", "hurlBoulder" + this.SimplifiedName, Divine.Input.Key.None, true);
        this.AstralPulseKey = new MenuHoldKey("Astra pulse", "astraPulse" + this.SimplifiedName, Divine.Input.Key.None, true);

        this.Menu.Add(this.DispelMagicKey);
        this.Menu.Add(this.CycloneKey);
        this.Menu.Add(this.WindWalkKey);
        this.Menu.Add(this.HurlBoulder);
        this.Menu.Add(this.AstralPulseKey);
        rootMenu.Add(this.Menu);
    }

    public MenuHoldKey DispelMagicKey { get; }

    public MenuHoldKey CycloneKey { get; }

    public MenuHoldKey WindWalkKey { get; }

    public MenuHoldKey HurlBoulder { get; }

    public MenuHoldKey AstralPulseKey { get; }
}
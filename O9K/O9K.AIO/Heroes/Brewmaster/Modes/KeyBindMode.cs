namespace O9K.AIO.Heroes.Brewmaster.Modes;

using System;
using System.Linq;

using Divine.Game;
using Divine.Update;

using O9K.AIO.Heroes.Base;
using O9K.AIO.Heroes.Brewmaster.Units;
using O9K.AIO.Modes.Base;
using O9K.AIO.UnitManager;
using O9K.Core.Logger;

using Void = Units.Void;

internal sealed class KeyBindMode : BaseMode
{
    private readonly KeyBindModeMenu menu;

    public KeyBindMode(BaseHero baseHero, KeyBindModeMenu menu)
        : base(baseHero)
    {
        this.UnitManager = baseHero.UnitManager;
        this.menu = menu;

        this.UpdateHandler = UpdateManager.CreateIngameUpdate(0, false, this.OnUpdate);
    }

    private IUnitManager UnitManager { get; }

    private UpdateHandler UpdateHandler { get; }

    public void Disable()
    {
        this.UpdateHandler.IsEnabled = false;
    }

    public override void Dispose()
    {
        base.Dispose();

        this.UpdateHandler.IsEnabled = false;
    }

    public void Enable()
    {
        this.UpdateHandler.IsEnabled = true;
    }

    private void ExecuteCombo()
    {
        if (!this.TargetManager.HasValidTarget)
        {
            return;
        }

        if (menu.DispelMagicKey)
        {
            ((Storm)this.UnitManager.ControllableUnits.FirstOrDefault(x => x is Storm))?.DispelMagicKeyBind(this.TargetManager);
        }
        else if (menu.CycloneKey)
        {
            ((Storm)this.UnitManager.ControllableUnits.FirstOrDefault(x => x is Storm))?.CycloneKeyBind(this.TargetManager);
        }
        else if (menu.WindWalkKey)
        {
            ((Storm)this.UnitManager.ControllableUnits.FirstOrDefault(x => x is Storm))?.WindWalkKeyBind();
        }
        else if (menu.HurlBoulder)
        {
            ((Earth)this.UnitManager.ControllableUnits.FirstOrDefault(x => x is Earth))?.HurlBoulderKeyBind(this.TargetManager);
        }
        else if (menu.AstralPulseKey)
        {
            ((Void)this.UnitManager.ControllableUnits.FirstOrDefault(x => x is Void))?.AstralPulseKeyBind();
        }
    }

    private void OnUpdate()
    {
        if (GameManager.IsPaused)
        {
            return;
        }

        try
        {
            this.ExecuteCombo();
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}
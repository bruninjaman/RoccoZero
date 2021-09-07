﻿namespace O9K.AIO.Heroes.StormSpirit;

using Base;

using Core.Entities.Metadata;

using Divine.Entity.Entities.Units.Heroes.Components;

using Modes;

[HeroId(HeroId.npc_dota_hero_storm_spirit)]
internal class StormSpiritBase : BaseHero
{
    private readonly ManaCalculatorMode manaCalculatorMode;

    private readonly OverloadChargeMode overloadChargeMode;

    public StormSpiritBase()
    {
        this.manaCalculatorMode = new ManaCalculatorMode(
            this,
            new ManaCalculatorModeMenu(this.Menu.RootMenu, "Mana calculator"));

        this.overloadChargeMode = new OverloadChargeMode(
            this,
            new OverloadChargeModeMenu(this.Menu.RootMenu, "Overload charge", "Use ball lighting to charge overload"));
    }

    public override void Dispose()
    {
        base.Dispose();
        this.manaCalculatorMode.Dispose();
        this.overloadChargeMode.Dispose();
    }

    protected override void DisableCustomModes()
    {
        this.manaCalculatorMode.Disable();
        this.overloadChargeMode.Disable();
    }

    protected override void EnableCustomModes()
    {
        this.manaCalculatorMode.Enable();
        this.overloadChargeMode.Enable();
    }
}
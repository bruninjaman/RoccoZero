﻿namespace O9K.AIO.Heroes.EarthSpirit;

using Base;

using Core.Entities.Metadata;

using Divine.Entity.Entities.Units.Heroes.Components;

using Modes;

[HeroId(HeroId.npc_dota_hero_earth_spirit)]
internal class EarthSpiritBase : BaseHero
{
    private readonly RollSmashMode rollSmashMode;

    public EarthSpiritBase()
    {
        this.rollSmashMode = new RollSmashMode(this, new RollSmashModeMenu(this.Menu.RootMenu, "Roll+Smash combo"));
    }

    public override void Dispose()
    {
        base.Dispose();
        this.rollSmashMode.Dispose();
    }

    protected override void DisableCustomModes()

    {
        this.rollSmashMode.Disable();
    }

    protected override void EnableCustomModes()
    {
        this.rollSmashMode.Enable();
    }
}
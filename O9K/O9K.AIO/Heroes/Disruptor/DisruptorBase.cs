﻿namespace O9K.AIO.Heroes.Disruptor;

using AIO.Modes.Permanent;

using Base;

using Core.Entities.Metadata;

using Divine.Entity.Entities.Units.Heroes.Components;

using Modes;

[HeroId(HeroId.npc_dota_hero_disruptor)]
internal class DisruptorBase : BaseHero
{
    private readonly GlimpseTrackerMode glimpseTrackerMode;

    public DisruptorBase()
    {
        this.glimpseTrackerMode = new GlimpseTrackerMode(this, new PermanentModeMenu(this.Menu.RootMenu, "Glimpse tracker"));
    }

    public override void Dispose()
    {
        base.Dispose();
        this.glimpseTrackerMode.Dispose();
    }

    protected override void DisableCustomModes()
    {
        this.glimpseTrackerMode.Disable();
    }

    protected override void EnableCustomModes()
    {
        this.glimpseTrackerMode.Enable();
    }
}
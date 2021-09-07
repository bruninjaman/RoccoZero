﻿namespace O9K.AIO.Heroes.Pudge;

using AIO.Modes.KeyPress;

using Base;

using Core.Entities.Metadata;

using Divine.Entity.Entities.Units.Heroes.Components;

using Modes;

[HeroId(HeroId.npc_dota_hero_pudge)]
internal class PudgeBase : BaseHero
{
    private readonly HookAllyMode hookAllyMode;

    public PudgeBase()
    {
        this.hookAllyMode = new HookAllyMode(this, new KeyPressModeMenu(this.Menu.RootMenu, "Hook ally"));
    }

    public override void Dispose()
    {
        base.Dispose();
        this.hookAllyMode.Dispose();
    }

    protected override void DisableCustomModes()
    {
        this.hookAllyMode.Disable();
    }

    protected override void EnableCustomModes()
    {
        this.hookAllyMode.Enable();
    }
}
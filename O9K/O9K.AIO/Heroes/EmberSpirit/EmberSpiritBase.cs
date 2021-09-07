﻿namespace O9K.AIO.Heroes.EmberSpirit;

using Base;

using Core.Entities.Metadata;

using Divine.Entity.Entities.Units.Heroes.Components;

using Modes;

[HeroId(HeroId.npc_dota_hero_ember_spirit)]
internal class EmberSpiritBase : BaseHero
{
    private readonly AutoChainsMode autoChainsMode;

    public EmberSpiritBase()
    {
        this.autoChainsMode = new AutoChainsMode(
            this,
            new AutoChainsModeMenu(this.Menu.RootMenu, "Auto chains", "Hold \"w\" to auto cast chains when using fist manually"));
    }

    public override void Dispose()
    {
        base.Dispose();
        this.autoChainsMode.Dispose();
    }

    protected override void DisableCustomModes()
    {
        this.autoChainsMode.Disable();
    }

    protected override void EnableCustomModes()
    {
        this.autoChainsMode.Enable();
    }
}
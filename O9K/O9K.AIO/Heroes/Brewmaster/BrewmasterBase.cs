namespace O9K.AIO.Heroes.Brewmaster;

using AIO.Modes.KeyPress;

using Base;

using Core.Entities.Metadata;

using Divine.Entity.Entities.Units.Heroes.Components;

using Modes;

[HeroId(HeroId.npc_dota_hero_brewmaster)]
internal class BrewmasterBase : BaseHero
{
    private readonly CycloneMode cycloneMode;

    private readonly KeyBindMode keyBindMode;

    public BrewmasterBase()
    {
        this.cycloneMode = new CycloneMode(
            this,
            new KeyPressModeMenu(this.Menu.RootMenu, "Cyclone enemy", "Use storm's cyclone on the target"));

        this.keyBindMode = new KeyBindMode(
            this,
            new KeyBindModeMenu(this.Menu.RootMenu, "Key binds"));
    }

    public override void Dispose()
    {
        base.Dispose();
        this.cycloneMode.Dispose();
        this.cycloneMode.Dispose();
    }

    protected override void DisableCustomModes()
    {
        this.cycloneMode.Disable();
        this.keyBindMode.Disable();
    }

    protected override void EnableCustomModes()
    {
        this.cycloneMode.Enable();
        this.keyBindMode.Enable();
    }
}
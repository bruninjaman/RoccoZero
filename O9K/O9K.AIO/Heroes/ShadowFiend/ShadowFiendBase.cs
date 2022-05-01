namespace O9K.AIO.Heroes.ShadowFiend;

using Base;

using Core.Entities.Metadata;
using Core.Managers.Menu.Items;

using Divine.Entity.Entities.Units.Heroes.Components;

using Utils;

[HeroId(HeroId.npc_dota_hero_nevermore)]
internal class ShadowFiendBase : BaseHero
{
    public static MenuSwitcher razeToMouseSwitcher;

    public static MenuSelector colourSelector;

    public static MenuSwitcher drawRazesSwitcher;

    public ShadowFiendBase()
    {
        var razeMenu = this.Menu.RootMenu.Add(new Menu("Raze settings"));

        razeToMouseSwitcher = razeMenu.Add(new MenuSwitcher("Raze to mouse"));
        drawRazesSwitcher = razeMenu.Add(new MenuSwitcher("Draw razes"));
        colourSelector = razeMenu.Add(new MenuSelector("Colour", RazeUtils.Colours.Keys));
        RazeUtils.Init(this.TargetManager, this.Menu);
    }

}
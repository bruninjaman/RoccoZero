using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Menu;
using Divine.Menu.Items;
using InvokerAnnihilation.Feature.ComboConstructor;
using InvokerAnnihilation.Feature.ComboExecutor;

namespace InvokerAnnihilation.Config;

public sealed class MenuConfig
{
    public readonly ComboConstructorMenu ComboConstructorMenu;
    public readonly ComboExecutorMenu ComboExecutorMenu;
    public readonly RootMenu RootMenu;

    public MenuConfig()
    {
        RootMenu = MenuManager
            .CreateRootMenu("Invoker Annihilation")
            .SetHeroImage(HeroId.npc_dota_hero_invoker);

        ComboConstructorMenu = new ComboConstructorMenu(RootMenu);
        ComboExecutorMenu = new ComboExecutorMenu(RootMenu);
    }
}
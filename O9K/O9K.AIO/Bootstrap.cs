namespace O9K.AIO;

using System;
using System.Reflection;

using Core.Entities.Metadata;
using Core.Logger;
using Core.Managers.Entity;

using Divine.Service;

using Heroes.Base;

using O9K.Core.Managers.Context;

using MenuCore = Core.Managers.Menu.Items.Menu;

//[ExportPlugin("O9K // AIO", priority: int.MaxValue)]
public class Bootstrap : Bootstrapper
{
    private BaseHero hero;

    public static MenuCore RootMenu { get; private set; }

    protected override void OnMainActivate()
    {
        RootMenu = new MenuCore("AIO", "O9K.AIO");

        Context9.MenuManager.AddRootMenu(RootMenu);
    }

    protected override void OnActivate()
    {
        try
        {
            var owner = EntityManager9.Owner;
            var type = Array.Find(
                Assembly.GetExecutingAssembly().GetTypes(),
                x => !x.IsAbstract && x.IsClass && typeof(BaseHero).IsAssignableFrom(x)
                     && x.GetCustomAttribute<HeroIdAttribute>()?.HeroId == owner.HeroId);

            if (type == null)
            {
                Logger.Warn("O9K.AIO // Hero is not supported");
                Logger.Warn("O9K.AIO // Dynamic combo will be loaded");

                this.hero = new BaseHero();
                return;
            }

            this.hero = (BaseHero)Activator.CreateInstance(type);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    protected override void OnMainDeactivate()
    {
        Context9.MenuManager.RemoveRootMenu(RootMenu);
    }

    protected override void OnDeactivate()
    {
        try
        {
            this.hero?.Dispose();
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}
using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Service;
using InvokerAnnihilation.Config;
using InvokerAnnihilation.Feature.ComboConstructor;
using InvokerAnnihilation.Feature.ComboExecutor;
using InvokerAnnihilation.Service;
using Microsoft.Extensions.DependencyInjection;

namespace InvokerAnnihilation;

public class Bootstrap : Bootstrapper
{
    private IServiceProvider? _serviceProvider;

    protected override void OnActivate()
    {
        if (EntityManager.LocalHero == null || EntityManager.LocalHero.HeroId != HeroId.npc_dota_hero_invoker)
        {
            return;
        }
        _serviceProvider = ServiceProviderBuilder.BuildServiceProvider();
        InitServices(_serviceProvider);
        Console.WriteLine("Bootstrap activate");
    }

    private static void InitServices(IServiceProvider serviceProvider)
    {
        serviceProvider.GetService<MenuConfig>();
        serviceProvider.GetService<ResourcesDownloader>();
        
        serviceProvider.GetService<ComboConstructorFeature>();
        serviceProvider.GetService<ComboExecutorFeature>();
    }

    protected override void OnDeactivate()
    {
    }
}
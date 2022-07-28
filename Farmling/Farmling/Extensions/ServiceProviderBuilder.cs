using Farmling.Config;
using Farmling.Interfaces;
using Farmling.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Farmling.Extensions;

public static class ServiceProviderBuilder
{
    public static IServiceProvider BuildServiceProvider()
    {
        var sc = new ServiceCollection();
        ConfigureServices(sc);
        return sc.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection sc)
    {
        sc.AddSingleton<MenuConfig>();
        sc.AddSingleton<Debugger>();

        sc.RegisterFeatures();
        sc.RegisterCombos();
    }

    private static void RegisterFeatures(this IServiceCollection sc)
    {
        sc.AddSingleton<IProjectileTrackManager, ProjectileTrackManager>();
        sc.AddSingleton<IHitsManager, HitsManager>();
        sc.AddSingleton<IFarmService, FarmService>();
    }

    private static void RegisterCombos(this IServiceCollection sc)
    {
    }
}

using InvokerAnnihilation.Abilities.AbilityManager;
using InvokerAnnihilation.Config;
using InvokerAnnihilation.Feature.ComboConstructor;
using InvokerAnnihilation.Feature.ComboConstructor.Interface;
using InvokerAnnihilation.Feature.ComboConstructor.Panels;
using InvokerAnnihilation.Feature.ComboExecutor;
using Microsoft.Extensions.DependencyInjection;

namespace InvokerAnnihilation.Service;

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
        sc.AddSingleton<ResourcesDownloader>();
        sc.AddSingleton<MenuConfig>();

        sc.AddSingleton<IAbilityManager, AbilityManager>();
        sc.AddSingleton<IComboInfo, ComboInfo>();
        
        sc.RegisterFeatures();
        sc.RegisterCombos();
    }

    private static void RegisterFeatures(this IServiceCollection sc)
    {
        sc.AddSingleton<ComboConstructorFeature>();
        sc.AddSingleton<ComboExecutorFeature>();
        sc.AddSingleton<IAbilityExecutor, AbilityExecutor>();
    }

    private static void RegisterCombos(this IServiceCollection sc)
    {
        sc.AddSingleton<IComboBuilder, CustomComboBuilder>();
        sc.AddSingleton<IComboBuilder, StandardComboBuilder>();

        // sc.AddSingleton<ComboConstructorMenu>();
        // sc.AddSingleton<ComboExecutorMenu>();
    }
}
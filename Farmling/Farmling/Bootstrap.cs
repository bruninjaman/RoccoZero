using Divine.Service;
using Farmling.Config;
using Farmling.Extensions;
using Farmling.Interfaces;
using Farmling.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Farmling;

public class Bootstrap : Bootstrapper
{
    private IServiceProvider? _serviceProvider;

    protected override void OnActivate()
    {
        _serviceProvider = ServiceProviderBuilder.BuildServiceProvider();
        InitServices(_serviceProvider);
    }

    private static void InitServices(IServiceProvider serviceProvider)
    {
        serviceProvider.GetService<MenuConfig>();
        serviceProvider.GetService<Debugger>();
        serviceProvider.GetService<IProjectileTrackManager>();
        serviceProvider.GetService<IHitsManager>();
    }

    protected override void OnDeactivate()
    {
    }
}

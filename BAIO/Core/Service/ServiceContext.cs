﻿namespace Ensage.SDK.Service;

using Divine.Entity;
using Divine.Entity.Entities.Units;

using Ensage.SDK.Orbwalker;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;

public class ServiceContext : IServiceContext
{
    public ServiceContext()
    {
        Owner = EntityManager.LocalHero;
        TargetSelector = new TargetSelectorManager();
        Orbwalker = new OrbwalkerManager();
    }

    public Unit Owner { get; }

    //public IAbilityDetector AbilityDetector { get; }

    //public AbilityFactory AbilityFactory { get; }

    //public IInventoryManager Inventory { get; }

    public IOrbwalkerManager Orbwalker { get; }

    //public IPredictionManager Prediction { get; }

    public ITargetSelectorManager TargetSelector { get; }

    public void Dispose()
    {
        TargetSelector.Dispose();
    }
}
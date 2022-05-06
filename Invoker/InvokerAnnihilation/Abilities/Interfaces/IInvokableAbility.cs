using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Processor;

namespace InvokerAnnihilation.Abilities.Interfaces;

public interface IInvokableAbility
{
    bool CanBeInvoked(bool andCasted = false);
    bool Invoke();
    bool IsInvoked { get; }
    AbilityId[] Spheres { get; }
    IInvokeProcessor Processor { get; }
}
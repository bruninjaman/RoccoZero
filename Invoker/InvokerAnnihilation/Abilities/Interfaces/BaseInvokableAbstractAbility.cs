using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.Processor;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseInvokableAbstractAbility : BaseAbstractAbility, IInvokableAbility
{
    protected BaseInvokableAbstractAbility(Ability baseAbility, AbilityId[] spheres) : base(baseAbility)
    {
        Spheres = spheres;
        Processor = new InvokeProcessor(this);
    }

    public bool CanBeInvoked(bool andCasted = false)
    {
        return Processor.CanBeInvoked(andCasted);
    }

    public bool Invoke()
    {
        return Processor.Invoke();
    }

    public bool IsInvoked => Processor.IsInvoked();
    public AbilityId[] Spheres { get; }
    public IInvokeProcessor Processor { get; }
}
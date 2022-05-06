namespace InvokerAnnihilation.Abilities.Processor;

public interface IInvokeProcessor
{
    bool Invoke();
    bool CanBeInvoked(bool andCasted);
    bool IsInvoked();
}
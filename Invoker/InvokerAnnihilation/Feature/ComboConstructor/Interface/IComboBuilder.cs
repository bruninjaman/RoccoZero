using InvokerAnnihilation.Feature.ComboConstructor.Combos;

namespace InvokerAnnihilation.Feature.ComboConstructor.Interface;

public interface IComboBuilder : IDisposable
{
    ComboBuildType Type { get; }
    void Render();
    StandardCombo? GetCurrentCombo();
}
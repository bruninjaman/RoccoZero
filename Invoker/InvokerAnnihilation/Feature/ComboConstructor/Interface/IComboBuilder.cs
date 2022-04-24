using InvokerAnnihilation.Feature.ComboConstructor.Combos;

namespace InvokerAnnihilation.Feature.ComboConstructor.Interface;

public interface IComboBuilder : IDisposable
{
    ComboBuildType Type { get; }
    void Render();
    ComboBase? GetCurrentCombo();
    DynamicCombo DynamicComboSettings { get; set; }
    CataclysmInCombo CataclysmInCombo { get; set; }
    void Start();
    void Stop();
}
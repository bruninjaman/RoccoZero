using Divine.Numerics;
using InvokerAnnihilation.Feature.ComboConstructor.Combos;
using InvokerAnnihilation.Feature.ComboConstructor.Combos.Dto;

namespace InvokerAnnihilation.Feature.ComboConstructor.Interface;

public interface ICombo : IScreenToggleItem
{
    public Dictionary<int, CustomComboStruct> Abilities { get; set; }
    public int ChangeIndex { get; set; }
}
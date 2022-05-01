using Divine.Numerics;
using InvokerAnnihilation.Feature.ComboConstructor.Interface;

namespace InvokerAnnihilation.Feature.ComboConstructor.Combos;

public class CataclysmInCombo : IScreenToggleItem
{
    public RectangleF ActivateBtnPosition { get; set; }
    public bool IsActive { get; set; }
}
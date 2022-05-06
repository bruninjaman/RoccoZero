using Divine.Numerics;

namespace InvokerAnnihilation.Feature.ComboConstructor.Interface;

public interface IScreenToggleItem 
{
    public RectangleF ActivateBtnPosition { get; set; }
    public bool IsActive { get; set; }
}
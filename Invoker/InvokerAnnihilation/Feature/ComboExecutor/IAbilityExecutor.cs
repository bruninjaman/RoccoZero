using Divine.Entity.Entities.Units.Heroes;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Feature.ComboExecutor;

public interface IAbilityExecutor
{
    bool CastAbility(IAbility ability, Hero target);
    // bool CastAbility(Vector3 position);
    // bool CastAbility();
}

public interface IComboInfo
{
    int ComboIndex { get; }
    bool IsInCombo { get; set; }
    void IncreaseComboIndex();
    void ResetComboIndex();
}

public class ComboInfo : IComboInfo
{
    public int ComboIndex { get; private set; }
    public bool IsInCombo { get; set; }

    public void IncreaseComboIndex()
    {
        ComboIndex++;
    }

    public void ResetComboIndex()
    {
        ComboIndex = 0;
    }
}
namespace InvokerAnnihilation.Feature.ComboExecutor;

public interface IComboInfo
{
    int ComboIndex { get; }
    bool IsInCombo { get; set; }
    void IncreaseComboIndex();
    void ResetComboIndex();
}
namespace InvokerAnnihilation.Feature.ComboExecutor;

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
namespace InvokerAnnihilation.Feature.ComboConstructor.Interface;

public class ComboBuilderFactory
{
    public ComboBuilderFactory(IEnumerable<IComboBuilder> comboBuilders)
    {
        Data = comboBuilders.ToDictionary(z => z.Type);
    }

    public Dictionary<ComboBuildType,IComboBuilder> Data { get; set; }
}
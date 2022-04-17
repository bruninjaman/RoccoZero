namespace InvokerAnnihilation.Feature.FeatureBase;

public interface IFeature<T> : IDisposable
{
    public T CurrentMenu { get; set; }
    void Enable();
    void Disable();
}
using Divine.Renderer;

namespace InvokerAnnihilation;

public class ResourcesDownloader
{
    public ResourcesDownloader()
    {
        // var extra = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
        foreach (var (key, value) in Files)
        {
            Console.WriteLine($"Loading custom resource: {value} as {key}");
            RendererManager.LoadImageFromAssembly(key, value);
        }
    }

    private Dictionary<string, string> Files { get; } = new()
    {
        {"RedCircle", "InvokerAnnihilation.Resources.RedCircle.png"},
        {"GrayCircle", "InvokerAnnihilation.Resources.GrayCircle.png"}
    };
}
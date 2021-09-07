namespace DotaMapPlus;

using Divine.Service;

internal sealed class DotaMapPlus : Bootstrapper
{
    private Config Config { get; set; }

    protected override void OnActivate()
    {
        Config = new Config();
    }

    /*protected override void OnDeactivate()
    {
        Config?.Dispose();
    }*/
}
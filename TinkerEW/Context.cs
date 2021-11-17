namespace TinkerEW;

internal sealed class Context
{
    public readonly Menu Menu;
    private readonly TinkerMain TinkerCombo;

    public Context()
    {
        Menu = new Menu();
        TinkerCombo = new TinkerMain(this);
    }

    internal void Dispose()
    {

    }
}
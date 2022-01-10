namespace O9K.Evader.Settings;

using Core.Managers.Menu.Items;

using Metadata;

internal class MenuManager : IEvaderService, IMainMenu
{
    public MenuManager(Menu menu)
    {
        this.EnemySettings = new EnemiesSettingsMenu(menu);
        this.AllySettings = new AlliesSettingsMenu(menu);
        this.AbilitySettings = new UsableAbilitiesMenu(menu);
        this.Settings = new SettingsMenu(menu);
        this.Hotkeys = new HotkeysMenu(menu);
        this.Debug = new DebugMenu(menu);
    }

    public UsableAbilitiesMenu AbilitySettings { get; private set; }

    public AlliesSettingsMenu AllySettings { get; private set; }

    public DebugMenu Debug { get; private set; }

    public EnemiesSettingsMenu EnemySettings { get; private set; }

    public HotkeysMenu Hotkeys { get; private set; }

    public LoadOrder LoadOrder { get; private set; } = LoadOrder.Settings;

    public SettingsMenu Settings { get; private set; }

    public void Activate()
    {
    }

    public void Dispose()
    {
    }
}
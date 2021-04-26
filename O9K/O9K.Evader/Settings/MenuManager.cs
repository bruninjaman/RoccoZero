namespace O9K.Evader.Settings
{

    using Core.Managers.Menu;
    using Core.Managers.Menu.Items;

    using Divine;

    using Metadata;

    using O9K.Core.Managers.Context;

    internal class MenuManager : IEvaderService, IMainMenu
    {
        private readonly IMenuManager9 menuManager;

        private readonly Menu menu;

        public MenuManager()
        {
            this.menuManager = Context9.MenuManager;

            this.menu = new Menu("Evader", "O9K.Evader").SetTexture(AbilityId.techies_minefield_sign);

            this.EnemySettings = new EnemiesSettingsMenu(this.menu);
            this.AllySettings = new AlliesSettingsMenu(this.menu);
            this.AbilitySettings = new UsableAbilitiesMenu(this.menu);
            this.Settings = new SettingsMenu(this.menu);
            this.Hotkeys = new HotkeysMenu(this.menu);
            this.Debug = new DebugMenu(this.menu);
        }

        public UsableAbilitiesMenu AbilitySettings { get; }

        public AlliesSettingsMenu AllySettings { get; }

        public DebugMenu Debug { get; }

        public EnemiesSettingsMenu EnemySettings { get; }

        public HotkeysMenu Hotkeys { get; }

        public LoadOrder LoadOrder { get; } = LoadOrder.Settings;

        public SettingsMenu Settings { get; }

        public void Activate()
        {
            this.menuManager.AddRootMenu(this.menu);
        }

        public void Dispose()
        {
            this.menuManager.RemoveRootMenu(this.menu);
        }
    }
}
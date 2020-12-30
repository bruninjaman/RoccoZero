using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.PartialMapHack
{
    internal class PartialMapHackMenu
    {
        public PartialMapHackMenu(RootMenu rootMenu)
        {
            var partialMapHackMenu = rootMenu.CreateMenu("Partial MapHack");
            SpellsItem = partialMapHackMenu.CreateSwitcher("Spells");
            ItemsItem = partialMapHackMenu.CreateSwitcher("Items");
            SmokeItem = partialMapHackMenu.CreateSwitcher("Smoke");
            TeleportItem = partialMapHackMenu.CreateSwitcher("Teleport");
            ModifersItem = partialMapHackMenu.CreateSwitcher("Modifers");
            WhenIsVisibleItem = partialMapHackMenu.CreateSwitcher("When Is Visible").SetTooltip("Dangerous Ability When Is Visible");
            SideMessageItem = partialMapHackMenu.CreateSwitcher("Side Message").SetTooltip("Dangerous Ability Side Message");
            SoundItem = partialMapHackMenu.CreateSwitcher("Play Sound").SetTooltip("Dangerous Ability Play Sound");
            OnMinimapItem = partialMapHackMenu.CreateSwitcher("Draw On Minimap");
            OnWorldItem = partialMapHackMenu.CreateSwitcher("Draw On World");
            ReduceTheNumberOfIconsItem = partialMapHackMenu.CreateSwitcher("Reduce The Number Of Icons", false);
            DrawTimerItem = partialMapHackMenu.CreateSlider("Draw Timer", 5, 1, 9);
        }

        public MenuSwitcher SpellsItem { get; }

        public MenuSwitcher ItemsItem { get; }

        public MenuSwitcher SmokeItem { get; }

        public MenuSwitcher TeleportItem { get; }

        public MenuSwitcher ModifersItem { get; }

        public MenuSwitcher WhenIsVisibleItem { get; }

        public MenuSwitcher SideMessageItem { get; }

        public MenuSwitcher SoundItem { get; }

        public MenuSwitcher OnMinimapItem { get; }

        public MenuSwitcher OnWorldItem { get; }

        public MenuSwitcher ReduceTheNumberOfIconsItem { get; }

        public MenuSlider DrawTimerItem { get; }
    }
}
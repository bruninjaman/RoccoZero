using Divine.GameConsole;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

namespace DotaMapPlus
{
    internal class ConsoleCommands
    {
        private MenuSwitcher FogItem { get; }

        private MenuSwitcher FilteringItem { get; }

        private MenuSwitcher ParticleHackItem { get; }

        public ConsoleCommands(RootMenu rootMenu)
        {
            var consoleCommandsMenu = rootMenu.CreateMenu("Console Commands");
            FogItem = consoleCommandsMenu.CreateSwitcher("Fog Disable");
            FilteringItem = consoleCommandsMenu.CreateSwitcher("Filtering Disable");
            ParticleHackItem = consoleCommandsMenu.CreateSwitcher("Particle Hack Enable");

            FogItem.ValueChanged += FogItemChanged;
            FilteringItem.ValueChanged += FilteringItemChanged;
            ParticleHackItem.ValueChanged += ParticleHackItemChanged;
        }

        /*public void Dispose()
        {
            Fog.SetValue(1);
            Filtering.SetValue(0);
            ParticleHack.SetValue(1);

            FogItem.PropertyChanged -= FogItemChanged;
            FilteringItem.PropertyChanged -= FilteringItemChanged;
            ParticleHackItem.PropertyChanged -= ParticleHackItemChanged;
        }*/

        private void FogItemChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            GameConsoleManager.SetValue("fog_enable", !e.Value);
        }

        private void FilteringItemChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            GameConsoleManager.SetValue("fow_client_nofiltering", e.Value);
        }

        private void ParticleHackItemChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            GameConsoleManager.SetValue("dota_use_particle_fow", !e.Value);
        }
    }
}
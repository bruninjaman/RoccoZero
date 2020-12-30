using System;

namespace Divine.Plugin.DotaMap
{
    internal sealed class ConsoleCommands
    {
        public ConsoleCommands(Config config)
        {
            ConVarManager.SetValue("fog_enable", !config.FogItem);
            ConVarManager.SetValue("fow_client_nofiltering", config.FilteringItem);
            ConVarManager.SetValue("dota_use_particle_fow", !config.ParticleHackItem);
        }
    }
}
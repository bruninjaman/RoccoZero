namespace O9K.Hud.Helpers
{
    using System.Collections.Generic;

    using Divine;

    using Modules;

    internal class ConsoleCommands : IHudModule
    {
        private readonly Dictionary<string, int> consoleCommands = new Dictionary<string, int>
        {
            { "dota_use_particle_fow", 0 },
            { "fog_enable", 0 },
        };

        public void Activate()
        {
            foreach (var cmd in this.consoleCommands)
            {
                ConVarManager.SetValue(cmd.Key, cmd.Value);
            }
        }

        public void Dispose()
        {
        }
    }
}
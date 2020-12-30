using System.IO;

using Divine.Zero.Helpers;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Divine.Plugin.DotaMap
{
    internal sealed class Config
    {
        public bool FogItem { get; } = true;

        public bool FilteringItem { get; } = true;

        public bool ParticleHackItem { get; } = true;

        public int ZoomItem { get; } = 1500;

        public int WeatherItem { get; } = 0;

        public Config()
        {
            var configFile = Path.Combine(Directories.Config, "DotaMapConfig.json");
            if (!File.Exists(configFile))
            {
                using var sw = File.CreateText(configFile);
                sw.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            else
            {
                var token = JsonConvert.DeserializeObject<JToken>(File.ReadAllText(configFile));
                FogItem = (bool)token["FogItem"];
                FilteringItem = (bool)token["FilteringItem"];
                ParticleHackItem = (bool)token["ParticleHackItem"];
                ZoomItem = (int)token["ZoomItem"];
                WeatherItem = (int)token["WeatherItem"];
            }
        }
    }
}
namespace Divine.Plugin.DotaMap
{
    internal sealed class Weather
    {
        public Weather(Config config)
        {
            ConVarManager.SetValue("cl_weather", config.WeatherItem);
        }
    }
}
using System;

using Divine;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

namespace DotaMapPlus
{
    internal class WeatherHack
    {
        private MenuSelector WeatherItem { get; }

        public WeatherHack(RootMenu rootMenu)
        {
            var weatherHackMenu = rootMenu.CreateMenu("Weather Hack");
            WeatherItem = weatherHackMenu.CreateSelector("Selected", WeatherNames);
            WeatherItem.ValueChanged += OnWeatherValueChanged;
        }

        /*public void Dispose()
        {
            Weather.SetValue(0);

            WeatherItem.PropertyChanged -= WeatherItemChanged;
        }*/

        private void OnWeatherValueChanged(MenuSelector selector, SelectorEventArgs e)
        {
            ConVarManager.SetValue("cl_weather", Array.IndexOf(WeatherNames, e.NewValue));
        }

        private readonly string[] WeatherNames =
        {
            "Default",
            "Snow",
            "Rain",
            "Moonbeam",
            "Pestilence",
            "Harvest",
            "Sirocco",
            "Ash",
            "Aurora"
        };
    }
}
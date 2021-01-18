using System;
using System.Windows.Input;

using Divine;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

namespace DotaMapPlus
{
    internal class ZoomHack
    {
        private MenuHoldKey ZoomKeyItem { get; }

        private MenuSlider ZoomSliderItem { get; }

        private int DefaultZoomValue { get; }

        private int MaxZoomValue { get; }

        private int MinZoomValue { get; }

        public ZoomHack(RootMenu rootMenu)
        {
            var zoomHackMenu = rootMenu.CreateMenu("Zoom Hack");
            ZoomKeyItem = zoomHackMenu.CreateHoldKey("Key", Key.LeftCtrl);
            ZoomSliderItem = zoomHackMenu.CreateSlider("Camera Distance", DefaultZoomValue, MinZoomValue, MaxZoomValue);

            ConVarManager.SetValue("r_farz", 18000);

            ZoomSliderItem.ValueChanged += OnZoomSliderValueChanged;
            InputManager.MouseWheel += OnInputManagerMouseWheel;

            GameManager.ExecuteCommand("dota_camera_disable_zoom true");
        }

        /*public void Dispose()
        {
            Game.ExecuteCommand("dota_camera_disable_zoom false");

            ZoomVar.SetValue(DefaultZoomValue);

            ZoomSliderItem.PropertyChanged -= ZoomSliderItemChanged;
            InputManage.Value.MouseWheel -= InputManagerMouseWheel;
        }*/

        private void OnZoomSliderValueChanged(MenuSlider slider, SliderEventArgs e)
        {
            ConVarManager.SetValue("dota_camera_distance", e.NewValue);
        }

        private void OnInputManagerMouseWheel(MouseWheelEventArgs e)
        {
            if (ZoomKeyItem)
            {
                var zoomValue = ConVarManager.GetInt32("dota_camera_distance");

                if (e.Up)
                {
                    zoomValue -= 50;
                    zoomValue = Math.Max(zoomValue, MinZoomValue);
                }
                else
                {
                    zoomValue += 50;
                    zoomValue = Math.Min(zoomValue, MaxZoomValue);
                }

                ZoomSliderItem.Value = zoomValue;
            }
        }
    }
}
namespace DotaMapPlus;

using System;

using Divine.GameConsole;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

internal class ZoomHack
{
    private MenuHoldKey ZoomKeyItem { get; }

    private MenuSlider ZoomSliderItem { get; }

    private int DefaultZoomValue { get; } = 1200;

    private int MaxZoomValue { get; } = 9000;

    private int MinZoomValue { get; } = 1134;

    public ZoomHack(RootMenu rootMenu)
    {
        var zoomHackMenu = rootMenu.CreateMenu("Zoom Hack");
        ZoomKeyItem = zoomHackMenu.CreateHoldKey("Key", System.Windows.Input.Key.LeftCtrl);
        ZoomSliderItem = zoomHackMenu.CreateSlider("Camera Distance", DefaultZoomValue, MinZoomValue, MaxZoomValue);

        GameConsoleManager.SetValue("r_farz", 18000);

        ZoomSliderItem.ValueChanged += OnZoomSliderValueChanged;
        InputManager.MouseWheel += OnInputManagerMouseWheel;

        GameConsoleManager.ExecuteCommand("dota_camera_disable_zoom true");
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
        GameConsoleManager.SetValue("dota_camera_distance", e.NewValue);
    }

    private void OnInputManagerMouseWheel(MouseWheelEventArgs e)
    {
        if (ZoomKeyItem)
        {
            var zoomValue = GameConsoleManager.GetInt32("dota_camera_distance");

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
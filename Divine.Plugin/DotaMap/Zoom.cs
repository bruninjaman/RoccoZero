namespace Divine.Plugin.DotaMap
{
    internal sealed class Zoom
    {
        public Zoom(Config config)
        {
            ConVarManager.SetValue("dota_camera_distance", config.ZoomItem);
            ConVarManager.SetValue("r_farz", 18000);
        }
    }
}
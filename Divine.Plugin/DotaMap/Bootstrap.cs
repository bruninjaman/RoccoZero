namespace Divine.Plugin.DotaMap
{
    internal sealed class Bootstrap
    {
        public Bootstrap()
        {
            var config = new Config();
            new ConsoleCommands(config);
            new Weather(config);
            new Zoom(config);

            System.Console.WriteLine("DotaMap Loaded");
        }
    }
}
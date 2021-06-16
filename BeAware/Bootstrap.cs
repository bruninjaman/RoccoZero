using System.Reflection;

using Divine.Renderer;
using Divine.Service;

namespace BeAware
{
    internal sealed class Bootstrap : Bootstrapper
    {
        private Common common;

        protected override void OnPreActivate()
        {
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (!resourceName.StartsWith("BeAware.Resources.Textures"))
                {
                    continue;
                }

                var stream = assembly.GetManifestResourceStream(resourceName);
                RendererManager.LoadImage(resourceName, stream);
            }
        }

        protected override void OnActivate()
        {
            common = new Common();
        }

        protected override void OnDeactivate()
        {
            common?.Dispose();
        }
    }
}
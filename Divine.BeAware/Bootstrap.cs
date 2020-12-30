using System.ComponentModel;
using System.Reflection;

namespace Divine.BeAware
{
    internal sealed class Bootstrap : Bootstrapper
    {
        private Common common;

        protected override void OnPreActivate()
        {
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (!resourceName.StartsWith("Divine.BeAware.Resources.Textures"))
                {
                    continue;
                }

                var stream = assembly.GetManifestResourceStream(resourceName);
                RendererManager.LoadTexture(resourceName, stream);
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
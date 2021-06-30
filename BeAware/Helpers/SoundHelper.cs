using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using BeAware.MenuManager;

using Divine.Log;
using Divine.Zero.Helpers;

namespace BeAware.Helpers
{
    internal sealed class SoundHelper
    {
        private readonly MenuConfig MenuConfig;

        private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

        private static readonly string[] ManifestResourceNames = ExecutingAssembly.GetManifestResourceNames();

        private const string ResourcesSounds = "BeAware.Resources.Sounds.";

        private static readonly string ResourceDirectory = Path.Combine(Directories.Cache, "Resource", "BeAware");

        public SoundHelper(Common common)
        {
            MenuConfig = common.MenuConfig;

            RuntimeHelpers.RunClassConstructor(typeof(SoundPlayer).TypeHandle);

            Task.Run(() =>
            {
                try
                {
                    foreach (var manifestResourceName in ManifestResourceNames)
                    {
                        if (!manifestResourceName.StartsWith(ResourcesSounds))
                        {
                            continue;
                        }

                        var file = new FileInfo(Path.Combine(ResourceDirectory, manifestResourceName.Substring(ResourcesSounds.Length)));
                        if (file.Exists)
                        {
                            continue;
                        }

                        var directory = file.Directory;
                        if (!directory.Exists)
                        {
                            directory.Create();
                        }

                        var stream = ExecutingAssembly.GetManifestResourceStream(manifestResourceName);
                        using var fileStream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write);
                        stream.CopyTo(fileStream);
                    }
                }
                catch (Exception e)
                {
                    LogManager.Error(e);
                }
            });
        }

        public void Play(string name)
        {
            if (MenuConfig.FullyDisableSoundsItem)
            {
                return;
            }

            try
            {
                string file;

                if (name.Contains("check_rune"))
                {
                    file = $"{name}_{MenuConfig.LanguageItem.Value.ToLower()}.wav";
                }
                else
                {
                    file = $"{name}.wav";
                }

                var volume = MenuConfig.VolumeItem.Value;

                if (MenuConfig.DefaultSoundItem)
                {
                    SoundPlayer.Play("default.wav", volume);
                    return;
                }

                if (!SoundPlayer.Play(file, volume))
                {
                    SoundPlayer.Play("default.wav", volume);
                }
            }
            catch (Exception e)
            {
                LogManager.Error(e);
            }
        }
    }
}
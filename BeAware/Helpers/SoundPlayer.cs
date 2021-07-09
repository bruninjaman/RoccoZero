using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Threading.Tasks;

//using DivineSoundPlayer = Divine.SoundPlayer.SoundPlayer;

namespace BeAware.Helpers
{
    public static class SoundPlayer
    {
        private static readonly Process CurrentProcess = Process.GetCurrentProcess();

        private static Task WaitHandler;

        private static readonly Dictionary<string, Stream> Sounds = new();

        static SoundPlayer()
        {
            var assembly = Assembly.GetExecutingAssembly();
            AssemblyLoadContext.GetLoadContext(assembly).LoadFromStream(assembly.GetManifestResourceStream("BeAware.Resources.Divine.SoundPlayer.dll"));

            InitializeSounds();
        }

        private static void InitializeSounds()
        {
            WaitHandler = Task.Run(() =>
            {
                /*var assembly = Assembly.GetExecutingAssembly();
                Sounds["check_rune_en.wav"] = DivineSoundPlayer.Decoder(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.check_rune_en.wav"));
                Sounds["check_rune_ru.wav"] = DivineSoundPlayer.Decoder(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.check_rune_ru.wav"));
                Sounds["default.wav"] = DivineSoundPlayer.Decoder(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.default.wav"));
                Sounds["item_smoke_of_deceit.wav"] = DivineSoundPlayer.Decoder(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.item_smoke_of_deceit.wav"));*/
            });
        }

        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        public static bool Play(string fileName, int volume)
        {
            if (!Sounds.TryGetValue(fileName, out var stream) || GetForegroundWindow() != CurrentProcess.MainWindowHandle)
            {
                return false;
            }

            Task.Run(async () =>
            {
                try
                {
                    await WaitHandler;

                    //DivineSoundPlayer.Play(stream, volume);
                }
                catch
                {
                }
            });

            return true;
        }
    }
}
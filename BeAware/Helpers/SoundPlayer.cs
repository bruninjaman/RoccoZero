namespace BeAware.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Threading.Tasks;

public static class SoundPlayer
{
    private static readonly Process CurrentProcess = Process.GetCurrentProcess();

    private static readonly Task WaitHandler;

    private static readonly Func<Stream, Stream> DecoderDelegate;

    private static readonly Action<Stream, float> PlayDelegate;

    private static readonly Dictionary<string, Stream> Sounds = new();

    static SoundPlayer()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var soundPlayerAssembly = AssemblyLoadContext.GetLoadContext(assembly).LoadFromStream(assembly.GetManifestResourceStream("BeAware.Resources.Divine.SoundPlayer.dll"));

        var soundPlayer = soundPlayerAssembly.GetType("Divine.SoundPlayer.SoundPlayer");
        DecoderDelegate = soundPlayer.GetMethod("Decoder").CreateDelegate<Func<Stream, Stream>>();
        PlayDelegate = soundPlayer.GetMethod("Play").CreateDelegate<Action<Stream, float>>();

        WaitHandler = Task.Run(() =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            Sounds["check_rune_en.wav"] = DecoderDelegate(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.check_rune_en.wav"));
            Sounds["check_rune_ru.wav"] = DecoderDelegate(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.check_rune_ru.wav"));
            Sounds["default.wav"] = DecoderDelegate(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.default.wav"));
            Sounds["item_smoke_of_deceit.wav"] = DecoderDelegate(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.item_smoke_of_deceit.wav"));
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

                PlayDelegate(stream, volume);
            }
            catch
            {
            }
        });

        return true;
    }
}
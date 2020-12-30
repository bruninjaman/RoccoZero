using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;

namespace Divine.BeAware.Helpers
{
    public static class SoundPlayer
    {
        private static readonly Process CurrentProcess = Process.GetCurrentProcess();

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        private static readonly MediaPlayer MediaPlayer = new() { Volume = 0 };

        public static bool Play(string file, int volume)
        {
            if (!File.Exists(file) || GetForegroundWindow() != CurrentProcess.MainWindowHandle)
            {
                return false;
            }

            MediaPlayer.Open(new Uri(file));
            MediaPlayer.Volume = volume * 0.01f;
            MediaPlayer.Play();

            return true;
        }
    }
}
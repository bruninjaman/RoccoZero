using Overwolf.Core;
using Overwolf.Renderer;

using System;

namespace Overwolf
{
    internal sealed class Context : IDisposable
    {
        public CoreMain CoreMain;
        public MainWindowOld RendererMainOld;
        public MainWindow RendererMain;
        public Menu Menu { get; }

        public Context()
        {
            Menu = new Menu();
            CoreMain = new CoreMain(this);
            //RendererMainOld = new MainWindowOld(this);
            RendererMain = new MainWindow(this);
        }

        public void Dispose()
        {
            CoreMain?.Dispose();
        }
    }
}
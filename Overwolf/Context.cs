using Overwolf.Core;
using Overwolf.Renderer;

using System;

namespace Overwolf
{
    internal sealed class Context
    {
        internal CoreMain CoreMain;
        internal MainWindow RendererMain;
        internal Menu Menu { get; }

        internal Context()
        {
            Menu = new Menu();
            CoreMain = new CoreMain(this);
            RendererMain = new MainWindow(this);
        }

        internal void Dispose()
        {
            CoreMain?.Dispose();
        }
    }
}
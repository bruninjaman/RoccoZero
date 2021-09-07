namespace Debugger.Tools;

using System;

internal interface IDebuggerTool : IDisposable
{
    int LoadPriority { get; }

    void Activate();
}
﻿using Divine.Service;

namespace TinkerEW;

public class BootStrap : Bootstrapper
{
    private Context? Context;

    protected override void OnActivate()
    {
        Context = new Context();
    }

    protected override void OnDeactivate()
    {
        Context?.Dispose();
    }
}
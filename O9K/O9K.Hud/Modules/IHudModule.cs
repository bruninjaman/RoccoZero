namespace O9K.Hud.Modules
{
    using System;

    internal interface IHudModule : IDisposable
    {
        void Activate();
    }
}
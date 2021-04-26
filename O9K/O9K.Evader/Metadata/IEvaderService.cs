namespace O9K.Evader.Metadata
{
    using System;

    internal interface IEvaderService : IDisposable
    {
        LoadOrder LoadOrder { get; }

        void Activate();
    }
}
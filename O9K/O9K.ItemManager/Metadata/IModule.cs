namespace O9K.ItemManager.Metadata
{
    using System;

    internal interface IModule : IDisposable
    {
        void Activate();
    }
}
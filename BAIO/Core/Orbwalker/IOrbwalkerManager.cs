namespace Ensage.SDK.Orbwalker
{
    using Ensage.SDK.Orbwalker.Modes;

    public interface IOrbwalkerManager : IOrbwalker
    {
        void RegisterMode(IOrbwalkingMode mode);

        void UnregisterMode(IOrbwalkingMode mode);

        void OnActivate();

        void OnDeactivate();
    }
}
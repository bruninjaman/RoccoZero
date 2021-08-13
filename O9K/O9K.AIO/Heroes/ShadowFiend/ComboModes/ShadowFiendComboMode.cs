namespace O9K.AIO.Heroes.ShadowFiend.ComboModes
{
    using System.Collections.Generic;

    using Base;

    using Modes.Combo;

    internal class ShadowFiendComboMode : ComboMode
    {
        public ShadowFiendComboMode(BaseHero baseHero, IEnumerable<ComboModeMenu> comboMenus) : base(baseHero, comboMenus)
        {
            Instance = this;
        }

        public static ShadowFiendComboMode Instance { get; set; }

        public static bool IsUpdateHandlerEnabled()
        {
            return Instance.UpdateHandler.IsEnabled;
        }
    }
}
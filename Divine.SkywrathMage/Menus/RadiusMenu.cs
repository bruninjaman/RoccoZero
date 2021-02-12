using System.ComponentModel;

using Divine.Core.ComboFactory.Menus;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class RadiusMenu : BaseRadiusMenu
    {
        [Item("Arcane Bolt")]
        [DefaultValue(true)]
        public bool ArcaneBoltItem { get; set; }

        [Item("Concussive Shot")]
        [DefaultValue(true)]
        public bool ConcussiveShotItem { get; set; }

        [Item("Ancient Seal")]
        [DefaultValue(true)]
        public bool AncientSealItem { get; set; }

        [Item("Mystic Flare")]
        [DefaultValue(true)]
        public bool MysticFlareItem { get; set; }

        [Item("Target Hit Concussive Shot")]
        [DefaultValue(true)]
        public bool TargetHitConcussiveShotItem { get; set; }

        [Item("Blink Dagger")]
        [DefaultValue(false)]
        public bool BlinkDaggerItem { get; set; }
    }
}

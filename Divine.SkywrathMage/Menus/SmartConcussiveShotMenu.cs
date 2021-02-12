using System.ComponentModel;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class SmartConcussiveShotMenu
    {
        [Item("Anti Fail")]
        [DefaultValue(true)]
        public bool AntiFailItem { get; set; }

        [Item("Use Only Target")]
        [Tooltip("This only works with Combo")]
        [DefaultValue(true)]
        public bool UseOnlyTargetItem { get; set; }

        [Item("Use In Radius")]
        [Tooltip("This only works with Combo")]
        public Slider<int> UseInRadiusItem { get; set; } = new Slider<int>(1400, 800, 1600);
    }
}

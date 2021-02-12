using System.ComponentModel;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Items;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class MysticFlareMenu
    {
        [Item("Target Min Health % To Ult:")]
        public Slider<int> MinHealthToUltItem { get; set; } = new Slider<int>(0, 0, 70);

        [Item(" ")]
        public string EmptyString { get; set; } = " ";

        [Item("Bad Ult")]
        [Tooltip("It is not recommended to enable this. If you do not have these items (RodofAtos, Hex, Ethereal) then this function is activated")]
        [DefaultValue(false)]
        public bool BadUltItem { get; set; }

        [Item("Bad Ult Movement Speed:")]
        [Tooltip("If the enemy has less Movement Speed from this value, then immediately ULT")]
        public Slider<int> BadUltMovementSpeedItem { get; set; } = new Slider<int>(500, 240, 500);
    }
}

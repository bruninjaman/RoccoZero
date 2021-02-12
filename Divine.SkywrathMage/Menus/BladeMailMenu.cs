using System.ComponentModel;

using Divine.Core.ComboFactory.Menus;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.ValueBinding;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class BladeMailMenu : BaseBladeMailMenu
    {
        [Item("Eul Control")]
        [Tooltip("Use Eul if there is BladeMail with ULT")]
        [DefaultValue(true)]
        public ValueType<bool> EulControlItem { get; set; } = new ValueType<bool>();
    }
}
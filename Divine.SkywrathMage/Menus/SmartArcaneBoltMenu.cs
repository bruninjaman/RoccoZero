using System.ComponentModel;
using System.Windows.Input;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Items;
using Ensage.SDK.Menu.ValueBinding;

using Newtonsoft.Json;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class SmartArcaneBoltMenu
    {
        public SmartArcaneBoltMenu()
        {
            ToggleHotkey.Action += ToggleHotkeyAction;
            SpamHotkey.Action += SpamHotkeyAction;
        }

        private void ToggleHotkeyAction(MenuInputEventArgs obj)
        {
            if (!ToggleHotkeyItem)
            {
                ToggleHotkeyItem = true;
                return;
            }

            ToggleHotkeyItem = false;
        }

        private void SpamHotkeyAction(MenuInputEventArgs agrs)
        {
            if (agrs.Flag == HotkeyFlags.Down)
            {
                SpamHotkeyItem.Value = true;
            }
            else
            {
                SpamHotkeyItem.Value = false;
            }
        }

        [JsonIgnore]
        public bool ToggleHotkeyItem { get; set; }

        [Item("Toggle Hotkey")]
        public HotkeySelector ToggleHotkey { get; set; } = new HotkeySelector(Key.None, HotkeyFlags.Press);

        [Item("Owner Min Health % To Auto Arcane Bolt:")]
        public Slider<int> OwnerMinHealthItem { get; set; } = new Slider<int>(20, 0, 70);

        [Item(" ")]
        public string EmptyString { get; set; } = " ";

        [JsonIgnore]
        public ValueType<bool> SpamHotkeyItem { get; set; } = new ValueType<bool>();

        [Item("Spam Hotkey")]
        public HotkeySelector SpamHotkey { get; set; } = new HotkeySelector(Key.None, HotkeyFlags.Up | HotkeyFlags.Down);

        [Item("Spam Units")]
        [DefaultValue(true)]
        public bool SpamUnitsItem { get; set; }

        [Item("Orbwalker")]
        public Selection<string> OrbwalkerItem { get; set; } = new Selection<string>(new[] { "Distance", "Default", "Free", "Only Attack", "No Move" });

        [Item("Min Distance In Orbwalk:")]
        public Slider<int> MinDisInOrbwalkItem { get; set; } = new Slider<int>(600, 200, 600);

        [Item("Full Free Mode")]
        [DefaultValue(false)]
        public bool FullFreeModeItem { get; set; }
    }
}

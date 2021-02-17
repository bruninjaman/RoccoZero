using System;

using Divine.Core.ComboFactory.Helpers;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.SkywrathMage.Menus;

using SharpDX;

namespace Divine.SkywrathMage.Helpers
{
    internal sealed class Renderer : BaseRenderer
    {
        private readonly AutoComboMenu AutoComboMenu;

        private readonly SmartArcaneBoltMenu SmartArcaneBoltMenu;

        public Renderer(Common common)
            : base(common)
        {
            var moreMenu = (MoreMenu)MenuConfig.MoreMenu;
            AutoComboMenu = moreMenu.AutoComboMenu;
            SmartArcaneBoltMenu = moreMenu.SmartArcaneBoltMenu;

            ComboPanelMore += 30;

            if (AutoComboMenu.EnableItem)
            {
                ComboPanelMore += 30;
            }

            AutoComboMenu.EnableItem.ValueChanged += AutoComboChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (AutoComboMenu.EnableItem)
            {
                ComboPanelMore -= 30;
            }

            AutoComboMenu.EnableItem.ValueChanged -= AutoComboChanged;
        }

        private void AutoComboChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                ComboPanelMore += 30;
            }
            else
            {
                ComboPanelMore -= 30;
            }
        }

        protected override void OnDraw(EventArgs args)
        {
            base.OnDraw(args);

            if (TextPanelMenu.ComboPanelItem)
            {
                var position = TextPanelMenu.Position.Value;

                var combo = ComboMenu.ComboHotkeyItem;
                var ownerHealth = ((float)Owner.Health / Owner.MaximumHealth) * 100;

                if (!combo && !SmartArcaneBoltMenu.SpamHotkeyItem && SmartArcaneBoltMenu.ToggleHotkeyItem && SmartArcaneBoltMenu.OwnerMinHealthItem.Value <= ownerHealth)
                {
                    Text($"Auto Q ON", position + new Vector2(0, 60), Color.Aqua);
                }
                else
                {
                    Text($"Auto Q OFF", position + new Vector2(0, 60), Color.Yellow);
                }

                if (AutoComboMenu.EnableItem)
                {
                    if (!AutoComboMenu.DisableWhenComboItem || !combo && AutoComboMenu.OwnerMinHealthItem.Value <= ownerHealth)
                    {
                        Text($"Auto Combo ON", position + new Vector2(0, 90), Color.Aqua);
                    }
                    else
                    {
                        Text($"Auto Combo OFF", position + new Vector2(0, 90), Color.Yellow);
                    }
                }
            }
        }
    }
}

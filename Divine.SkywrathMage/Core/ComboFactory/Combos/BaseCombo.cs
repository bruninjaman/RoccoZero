using System;

using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.SDK.Extensions;

using SharpDX;

namespace Divine.Core.ComboFactory.Combos
{
    public abstract class BaseCombo : BaseTaskHandler
    {
        private readonly BaseComboMenu ComboMenu;

        private readonly BaseWithMuteMenu WithMuteMenu;

        public BaseCombo(BaseMenuConfig menuConfig)
        {
            ComboMenu = menuConfig.ComboMenu;
            WithMuteMenu = ComboMenu.WithMuteMenu;

            ComboMenu.ComboHotkeyItem.Changed += ComboHotkeyChanged;
            WithMuteMenu.ComboHotkey.Action += WithMuteComboAction;
        }

        public override void Dispose()
        {
            WithMuteMenu.ComboHotkey.Action -= WithMuteComboAction;
            ComboMenu.ComboHotkeyItem.Changed -= ComboHotkeyChanged;
        }

        private void ComboHotkeyChanged(object sender, ValueChangingEventArgs<bool> e)
        {
            if (e.Value)
            {
                RunAsync();
            }
            else
            {
                Cancel();
            }
        }

        private void WithMuteComboAction(MenuInputEventArgs agrs)
        {
            if (agrs.Flag == HotkeyFlags.Down)
            {
                WithMuteMenu.ToggleHotkeyItem = true;
                RunAsync();
            }
            else
            {
                WithMuteMenu.ToggleHotkeyItem = false;
                Cancel();
            }
        }

        protected abstract bool WithMute(Unit target);

        protected void MoveTo(Vector3 position)
        {
            Orbwalker.MoveTo(position);
        }

        protected void MoveToMousePosition()
        {
            Orbwalker.MoveToMousePosition();
        }

        protected void OrbwalkTo(Unit target)
        {
            switch (ComboMenu.OrbwalkerItem.Value)
            {
                case "Default":
                    {
                        Orbwalker.OrbwalkTo(target, GameManager.MousePosition, false);
                    }
                    break;

                case "Distance":
                    {
                        var mousePosition = GameManager.MousePosition;
                        var ownerDis = Math.Min(Owner.Distance2D(mousePosition), 230);
                        var ownerPos = Owner.Position.Extend(mousePosition, ownerDis);
                        var position = target.Position.Extend(ownerPos, ComboMenu.MinDisInOrbwalkItem.Value);

                        if (ComboMenu.FullDistanceModeItem && Owner.Distance2D(target) <= ComboMenu.MinDisInOrbwalkItem.Value - 100)
                        {
                            MoveTo(position);
                        }
                        else
                        {
                            Orbwalker.OrbwalkTo(target, position, false);
                        }
                    }
                    break;

                case "Free":
                    {
                        var attackRange = Owner.AttackRange(target);
                        if (Owner.Distance2D(target) <= attackRange && !ComboMenu.FullFreeModeItem || target.Distance2D(GameManager.MousePosition) <= attackRange)
                        {
                            Orbwalker.OrbwalkTo(target, GameManager.MousePosition, false);
                        }
                        else
                        {
                            MoveToMousePosition();
                        }
                    }
                    break;

                case "Only Attack":
                    {
                        Orbwalker.AttackTo(target);
                    }
                    break;

                case "No Move":
                    {
                        if (Owner.Distance2D(target) < Owner.AttackRange(target))
                        {
                            Orbwalker.AttackTo(target);
                        }
                    }
                    break;
            }
        }
    }
}
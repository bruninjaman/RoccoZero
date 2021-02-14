using System;

using Divine.Core.ComboFactory.Commons;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.ComboFactory.Menus.Settings;
using Divine.Core.ComboFactory.Menus.Settings.Drawing;
using Divine.Core.ComboFactory.Menus.UnitMenus;
using Divine.Core.Entities;
using Divine.Core.Helpers;
using Divine.Core.Managers.Renderer;
using Divine.Core.Managers.TargetSelector;
using Divine.Core.Managers.Unit;


using Ensage.SDK.Menu.ValueBinding;

using SharpDX;

namespace Divine.Core.ComboFactory.Helpers
{
    public class BaseRenderer
    {
        protected readonly BaseMenuConfig MenuConfig;

        protected readonly BaseComboMenu ComboMenu;

        private readonly BaseWithMuteMenu WithMuteMenu;

        private readonly BaseUnitComboMenu UnitComboMenu;

        private readonly BaseUnitControlMenu UnitControlMenu;

        private readonly BaseUnitFarmMenu UnitFarmMenu;

        private readonly BaseSettingsMenu SettingsMenu;

        private readonly BaseDamageCalculationMenu DamageCalculationMenu;

        protected readonly BaseTextPanelMenu TextPanelMenu;

        private readonly TargetSelectorManager TargetSelector;

        private readonly PanelMove PanelMove;

        private Vector2 Size = new Vector2(150, 0);

        protected readonly CHero Owner = UnitManager.Owner;

        public BaseRenderer(BaseCommon common)
        {
            MenuConfig = common.MenuConfig;
            ComboMenu = MenuConfig.ComboMenu;
            WithMuteMenu = MenuConfig.ComboMenu.WithMuteMenu;
            UnitComboMenu = MenuConfig.UnitMenu.UnitComboMenu;
            UnitControlMenu = MenuConfig.UnitMenu.UnitControlMenu;
            UnitFarmMenu = MenuConfig.UnitMenu.UnitFarmMenu;
            SettingsMenu = MenuConfig.SettingsMenu;
            DamageCalculationMenu = SettingsMenu.DrawingMenu.DamageCalculationMenu;
            TextPanelMenu = common.MenuConfig.SettingsMenu.DrawingMenu.TextPanelMenu;

            TargetSelector = common.TargetSelector;

            PanelMove = new PanelMove(TextPanelMenu.Position.Value);

            if (!SettingsMenu.DisableDrawingItem)
            {
                RendererManager.TextureManager.LoadFromDivine(@"others\green_arrow.png");

                if (TextPanelMenu.ComboPanelItem)
                {
                    Size.Y += 60;
                }

                if (TextPanelMenu.UnitComboPanelItem)
                {
                    Size.Y += 90;
                }

                PanelMove.Size = Size;

                RendererManager.OnDraw += RendererOnDraw;
                Drawing.OnDraw += OnDraw;

                TextPanelMenu.MoveItem.Changed += MoveChanged;
                PanelMove.ValueChanged += PanelMoveChanged;

                TextPanelMenu.ComboPanelItem.Changed += ComboPanelChanged;
                TextPanelMenu.UnitComboPanelItem.Changed += UnitComboPanelChanged;
            }

            SettingsMenu.DisableDrawingItem.Changed += DisableChanged;
        }

        public virtual void Dispose()
        {
            if (!SettingsMenu.DisableDrawingItem)
            {
                TextPanelMenu.UnitComboPanelItem.Changed -= UnitComboPanelChanged;
                TextPanelMenu.ComboPanelItem.Changed -= ComboPanelChanged;
                
                PanelMove.ValueChanged -= PanelMoveChanged;
                TextPanelMenu.MoveItem.Changed -= MoveChanged;

                Drawing.OnDraw -= OnDraw;
                RendererManager.OnDraw -= RendererOnDraw;
            }

            TextPanelMenu.MoveItem.Changed -= MoveChanged;
        }

        private void DisableChanged(object sender, ValueChangingEventArgs<bool> e)
        {
            if (e.Value)
            {
                if (TextPanelMenu.ComboPanelItem)
                {
                    Size.Y += 60;
                }

                if (TextPanelMenu.UnitComboPanelItem)
                {
                    Size.Y += 90;
                }

                PanelMove.Size = Size;

                RendererManager.OnDraw += RendererOnDraw;
                Drawing.OnDraw += OnDraw;

                TextPanelMenu.MoveItem.Changed += MoveChanged;
                PanelMove.ValueChanged += PanelMoveChanged;

                TextPanelMenu.ComboPanelItem.Changed += ComboPanelChanged;
                TextPanelMenu.UnitComboPanelItem.Changed += UnitComboPanelChanged;
            }
            else
            {
                if (TextPanelMenu.ComboPanelItem)
                {
                    Size.Y -= 60;
                }

                if (TextPanelMenu.UnitComboPanelItem)
                {
                    Size.Y -= 90;
                }

                PanelMove.Size = Size;

                TextPanelMenu.UnitComboPanelItem.Changed -= UnitComboPanelChanged;
                TextPanelMenu.ComboPanelItem.Changed -= ComboPanelChanged;

                PanelMove.ValueChanged -= PanelMoveChanged;
                TextPanelMenu.MoveItem.Changed -= MoveChanged;

                Drawing.OnDraw -= OnDraw;
                RendererManager.OnDraw -= RendererOnDraw;
            }
        }

        private void MoveChanged(object sender, ValueChangingEventArgs<bool> e)
        {
            if (e.Value)
            {
                PanelMove.ActivateMove = true;
            }
            else
            {
                PanelMove.ActivateMove = false;
            }
        }

        private void PanelMoveChanged(bool isTimeout, Vector2 position)
        {
            if (isTimeout)
            {
                TextPanelMenu.MoveItem.Value = false;
                return;
            }

            TextPanelMenu.Position.Value = position;
        }

        private void ComboPanelChanged(object sender, ValueChangingEventArgs<bool> e)
        {
            if (e.Value)
            {
                Size.Y += 60;
            }
            else
            {
                Size.Y -= 60;
            }

            PanelMove.Size = Size;
        }

        private void UnitComboPanelChanged(object sender, ValueChangingEventArgs<bool> e)
        {
            if (e.Value)
            {
                Size.Y += 90;
            }
            else
            {
                Size.Y -= 90;
            }

            PanelMove.Size = Size;
        }

        protected virtual void RendererOnDraw()
        {
            if (UnitComboMenu.ToggleHotkeyItem)
            {
                var target = TargetSelector.UnitTarget;
                if (target != null)
                {
                    var topPanel = target.TopPanel;
                    if (topPanel != null)
                    {
                        var size = 50 * HUDInfo.RatioPercentage;
                        var midle = (topPanel.SizeX / 2) - (size / 2);
                        
                        var iconPosition = topPanel.IconPosition;
                        var position = new RectangleF(iconPosition.X + midle, iconPosition.Y + (110 * HUDInfo.RatioPercentage), size, size);
                        RendererManager.DrawTexture(@"others\green_arrow.png", position);
                    }
                }
            }
        }

        protected void Text(string text, Vector2 pos, Color color)
        {
            Drawing.DrawText(text, "Arial", pos, new Vector2(21), color, FontFlags.None);
        }

        private void Texture(Vector2 pos, Vector2 size, string texture)
        {
            Drawing.DrawRect(pos, size, Drawing.GetTexture($"materials/ensage_ui/{texture}.vmat"));
        }

        protected int comboPanelMore;

        protected int ComboPanelMore
        {
            get
            {
                return comboPanelMore;
            }

            set
            {
                Size.Y -= comboPanelMore;
                comboPanelMore = value;
                Size.Y += comboPanelMore;
                PanelMove.Size = Size;
            }
        }

        protected virtual void OnDraw(EventArgs args)
        {
            var comboPanelItem = TextPanelMenu.ComboPanelItem;
            var unitComboPanelItem = TextPanelMenu.UnitComboPanelItem;

            if (comboPanelItem || unitComboPanelItem)
            {
                var position = TextPanelMenu.Position.Value;
                if (PanelMove.ActivateMove)
                {
                    var movePosition = position - new Vector2(5);

                    var text = PanelMove.Time.ToString();
                    var textMinSize = new Vector2(Math.Min(Size.X, Size.Y));
                    var sizeText = Drawing.MeasureText(text, "Arial", textMinSize, FontFlags.AntiAlias);

                    Drawing.DrawText(text, "Arial", movePosition + ((Size / 2) - (sizeText / 2)), textMinSize, new Color(255, 255, 255, 50), FontFlags.AntiAlias);
                    Drawing.DrawRect(movePosition, Size, Color.WhiteSmoke, true);
                }

                var isCombo = ComboMenu.ComboHotkeyItem;
                if (comboPanelItem)
                {
                    if (isCombo)
                    {
                        Text($"Combo ON", position, Color.Aqua);
                    }
                    else
                    {
                        Text($"Combo OFF", position, Color.Yellow);
                    }

                    if (WithMuteMenu.ToggleHotkeyItem)
                    {
                        Text($"With Mute ON", position + new Vector2(0, 30), Color.Aqua);
                    }
                    else
                    {
                        Text($"With Mute OFF", position + new Vector2(0, 30), Color.Yellow);
                    }

                    position = position + new Vector2(0, 60 + ComboPanelMore);
                }

                if (unitComboPanelItem)
                {
                    if (UnitComboMenu.ToggleHotkeyItem)
                    {
                        Text("Unit Combo Lock", position, Color.Aqua);
                    }
                    else if (isCombo)
                    {
                        Text("Unit Combo ON", position, Color.Aqua);
                    }
                    else
                    {
                        Text("Unit Combo OFF", position, Color.Yellow);
                    }

                    if (UnitControlMenu.FollowToggleHotkeyItem)
                    {
                        Text($"Unit Follow ON", position + new Vector2(0, 30), Color.Aqua);
                    }
                    else
                    {
                        Text($"Unit Follow OFF", position + new Vector2(0, 30), Color.Yellow);
                    }

                    if (UnitFarmMenu.ToggleHotkeyItem)
                    {
                        Text($"Unit Farm {UnitFarmMenu.AreaItem}", position + new Vector2(0, 60), Color.Aqua);
                    }
                    else
                    {
                        Text($"Unit Farm OFF", position + new Vector2(0, 60), Color.Yellow);
                    }
                }
            }

            if (!SettingsMenu.DisableDamageCalculationItem)
            {
                foreach (var data in BaseDamageCalculation.DamageDate)
                {
                    var hero = data.GetHero;
                    var health = data.GetHealth;
                    var canBeCastedDamage = data.GetCanBeCastedDamage;

                    var hpBarPosition = HUDInfo.GetHpBarPosition(hero);
                    var hpBarSizeX = HUDInfo.HpBarSizeX;
                    var hpBarSizeY = HUDInfo.HpBarSizeY / 2.3f;
                    var hpBarPos = hpBarPosition + new Vector2(0, hpBarSizeY + 1.8f);

                    if (hpBarPosition.IsZero)
                    {
                        continue;
                    }

                    if (DamageCalculationMenu.HpBarItem)
                    {
                        var canHitDamage = data.GetCanHitdamage;
                        var maximumHealth = hero.MaximumHealth;

                        var canBeCastedDamageBar = Math.Max(canBeCastedDamage, 0) / maximumHealth;
                        if (canBeCastedDamageBar > 0)
                        {
                            var canBeCastedDamagePos = Math.Max(health - canBeCastedDamage, 0) / maximumHealth;
                            var canBeCastedDamagePosition = new Vector2(hpBarPos.X + ((hpBarSizeX + canBeCastedDamageBar) * canBeCastedDamagePos), hpBarPos.Y);
                            var canBeCastedDamageSize = new Vector2(hpBarSizeX * (canBeCastedDamageBar + Math.Min(health - canBeCastedDamage, 0) / maximumHealth), hpBarSizeY);
                            var canBeCastedDamageeColor = ((float)health / maximumHealth) - canBeCastedDamageBar > 0 ? new Color(100, 0, 0, 200) : new Color(191, 255, 0, 200);

                            Drawing.DrawRect(canBeCastedDamagePosition, canBeCastedDamageSize, canBeCastedDamageeColor);
                            Drawing.DrawRect(canBeCastedDamagePosition, canBeCastedDamageSize, Color.Black, true);
                        }

                        var canHitDamageBar = Math.Max(canHitDamage, 0) / maximumHealth;
                        if (canHitDamageBar > 0)
                        {
                            var canHitDamagePos = Math.Max(health - canHitDamage, 0) / maximumHealth;
                            var canHitDamagePosition = new Vector2(hpBarPos.X + ((hpBarSizeX + canHitDamageBar) * canHitDamagePos), hpBarPos.Y);
                            var canHitDamageSize = new Vector2(hpBarSizeX * (canHitDamageBar + Math.Min(health - canHitDamage, 0) / maximumHealth), hpBarSizeY);
                            var canHitDamageColor = ((float)health / maximumHealth) - canHitDamageBar > 0 ? new Color(0, 255, 0) : Color.Aqua;

                            Drawing.DrawRect(canHitDamagePosition, canHitDamageSize, canHitDamageColor);
                            Drawing.DrawRect(canHitDamagePosition, canHitDamageSize, Color.Black, true);
                        }
                    }

                    if (DamageCalculationMenu.ValueItem)
                    {
                        var damage = health - (int)canBeCastedDamage;
                        var color = damage > 0 ? Color.WhiteSmoke : Color.Aqua;
                        Drawing.DrawText($"{damage} ({(int)canBeCastedDamage})", "Calibri Bold", hpBarPosition + new Vector2(hpBarSizeX + (25 * HUDInfo.RatioPercentage), 0), new Vector2(17) * HUDInfo.RatioPercentage, color, FontFlags.AntiAlias);
                    }
                }
            }
        }
    }
}
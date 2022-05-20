﻿using System.Collections.Generic;

using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.ComboFactory.Menus.TargetSelector;
using Divine.Core.Managers.TargetSelector.TargetEffects.EffectType;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

namespace Divine.Core.Managers.TargetSelector.TargetEffects
{
    public class TargetEffectsManager
    {
        private TargetSelectorManager TargetSelectorManager { get; }

        private BaseComboMenu ComboMenu { get; }

        private TargetEffectsMenu TargetEffectsMenu { get; }

        private IEffectType EffectType { get; set; }

        private UpdateHandler UpdateHandler { get; }

        private readonly Dictionary<string, IEffectType> EffectTypeService = new Dictionary<string, IEffectType>
        {
            { "Default", new Default() },
            { "Without Circle", new WithoutCircle() }
        };

        public TargetEffectsManager(TargetSelectorManager targetSelectorManager)
        {
            TargetSelectorManager = targetSelectorManager;

            ComboMenu = TargetSelectorManager.ComboMenu;
            TargetEffectsMenu = TargetSelectorManager.TargetEffectsMenu;

            EffectType = EffectTypeService[TargetEffectsMenu.EffectTypeItem.Value];
            SetColor();

            TargetEffectsMenu.EffectTypeItem.ValueChanged += EffectTypeChanging;

            TargetEffectsMenu.EnableItem.ValueChanged += EnableChanged;
            TargetEffectsMenu.FreeEnableItem.ValueChanged += EnableChanged;

            TargetEffectsMenu.RedItem.ValueChanged += ColorChanging;
            TargetEffectsMenu.GreenItem.ValueChanged += ColorChanging;
            TargetEffectsMenu.BlueItem.ValueChanged += ColorChanging;
            TargetEffectsMenu.FreeRedItem.ValueChanged += ColorChanging;
            TargetEffectsMenu.FreeGreenItem.ValueChanged += ColorChanging;
            TargetEffectsMenu.FreeBlueItem.ValueChanged += ColorChanging;

            ComboMenu.ComboHotkeyItem.ValueChanged += ComboHotkeyChanged;

            UpdateHandler = UpdateManager.CreateIngameUpdate(20, TargetEffectsMenu.FreeEnableItem, OnUpdate);
        }

        public void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(OnUpdate);

            ComboMenu.ComboHotkeyItem.ValueChanged -= ComboHotkeyChanged;

            TargetEffectsMenu.EffectTypeItem.ValueChanged -= EffectTypeChanging;

            TargetEffectsMenu.EnableItem.ValueChanged -= EnableChanged;
            TargetEffectsMenu.FreeEnableItem.ValueChanged -= EnableChanged;

            TargetEffectsMenu.RedItem.ValueChanged -= ColorChanging;
            TargetEffectsMenu.GreenItem.ValueChanged -= ColorChanging;
            TargetEffectsMenu.BlueItem.ValueChanged -= ColorChanging;
            TargetEffectsMenu.FreeRedItem.ValueChanged -= ColorChanging;
            TargetEffectsMenu.FreeGreenItem.ValueChanged -= ColorChanging;
            TargetEffectsMenu.FreeBlueItem.ValueChanged -= ColorChanging;
        }

        private void SetColor()
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (ComboMenu.ComboHotkeyItem)
                {
                    EffectType.Red = TargetEffectsMenu.RedItem.Value;
                    EffectType.Green = TargetEffectsMenu.GreenItem.Value;
                    EffectType.Blue = TargetEffectsMenu.BlueItem.Value;
                    return;
                }

                EffectType.Red = TargetEffectsMenu.FreeRedItem.Value;
                EffectType.Green = TargetEffectsMenu.FreeGreenItem.Value;
                EffectType.Blue = TargetEffectsMenu.FreeBlueItem.Value;
            });
        }

        private void SetEffect()
        {
            SetEffect(ComboMenu.ComboHotkeyItem);
        }

        private void SetEffect(bool isCombo)
        {
            EffectType.Remove();

            if (isCombo)
            {
                UpdateHandler.IsEnabled = TargetEffectsMenu.EnableItem;
            }
            else
            {
                UpdateHandler.IsEnabled = TargetEffectsMenu.FreeEnableItem;
            }
        }

        private void EffectTypeChanging(MenuSelector selector, SelectorEventArgs e)
        {
            EffectType = EffectTypeService[e.NewValue];
            SetColor();
        }

        private void EnableChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            UpdateManager.BeginInvoke(() =>
            {
                SetEffect();
            });
        }

        private void ColorChanging(MenuSlider slider, SliderEventArgs e)
        {
            SetColor();
        }

        private void ComboHotkeyChanged(MenuHoldKey switcher, HoldKeyEventArgs e)
        {
            SetEffect(e.Value);
            SetColor();
        }

        public bool DisableTargetDraw { get; set; }

        private void OnUpdate()
        {
            var target = DisableTargetDraw ? null : TargetSelectorManager.Target;
            EffectType.Effect(target);
        }
    }
}

using System.Collections.Generic;

using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.SDK.Managers.Update;
using Divine.SkywrathMage.Menus;
using Divine.SkywrathMage.Menus.Combo;
using Divine.SkywrathMage.TargetSelector.TargetEffects.EffectType;

namespace Divine.SkywrathMage.TargetSelector.TargetEffects
{
    internal class TargetEffectsManager
    {
        private TargetSelectorManager TargetSelectorManager { get; }

        private ComboMenu ComboMenu { get; }

        private TargetEffectsMenu TargetEffectsMenu { get; }

        private IEffectType EffectType { get; set; }

        private UpdateHandler UpdateHandler { get; }

        private readonly Dictionary<string, IEffectType> EffectTypeService = new()
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

            UpdateHandler = UpdateManager.Subscribe(20, TargetEffectsMenu.FreeEnableItem, OnUpdate);
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);

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

        private void EffectTypeChanging(MenuSelector sender, SelectorEventArgs e)
        {
            EffectType = EffectTypeService[e.NewValue];
            SetColor();
        }

        private void EnableChanged(MenuSwitcher sender, SwitcherEventArgs e)
        {
            UpdateManager.BeginInvoke(() =>
            {
                SetEffect();
            });
        }

        private void ColorChanging(MenuSlider sender, SliderEventArgs e)
        {
            SetColor();
        }

        private void ComboHotkeyChanged(MenuHoldKey sender, HoldKeyEventArgs e)
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

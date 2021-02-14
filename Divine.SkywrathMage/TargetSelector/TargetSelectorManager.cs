using System.Collections.Generic;

using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.SDK.Managers.Update;
using Divine.SkywrathMage.Menus;
using Divine.SkywrathMage.Menus.Combo;
using Divine.SkywrathMage.TargetSelector.Selector;
using Divine.SkywrathMage.TargetSelector.TargetEffects;

namespace Divine.SkywrathMage.TargetSelector
{
    internal class TargetSelectorManager
    {
        public ComboMenu ComboMenu { get; } // TODO public

        private TargetSelectorMenu TargetSelectorMenu { get; }

        internal TargetEffectsMenu TargetEffectsMenu { get; }

        public TargetEffectsManager TargetEffectsManager { get; }

        public Hero Target { get; private set; }

        private ISelector Selector { get; set; }

        private readonly Dictionary<string, ISelector> SelectorService = new()
        {
            { "Lowest Health", new LowestHealth() },
            { "Near Mouse", new NearMouse() }
        };

        public TargetSelectorManager(MenuConfig menuConfig)
        {
            ComboMenu = menuConfig.ComboMenu;
            TargetSelectorMenu = menuConfig.TargetSelectorMenu;
            TargetEffectsMenu = TargetSelectorMenu.TargetEffectsMenu;

            Selector = SelectorService["Near Mouse"];

            TargetSelectorMenu.TargetSelectorItem.ValueChanged += TargetSelectorChanging;

            UpdateManager.Subscribe(40, OnUpdate);

            TargetEffectsManager = new TargetEffectsManager(this);
        }

        public void Dispose()
        {
            TargetEffectsManager.Dispose();

            UpdateManager.Unsubscribe(OnUpdate);

            TargetSelectorMenu.TargetSelectorItem.ValueChanged -= TargetSelectorChanging;
        }

        private void TargetSelectorChanging(MenuSelector sender, SelectorEventArgs e)
        {
            Selector = SelectorService[e.NewValue];
        }

        protected virtual void OnUpdate()
        {
            var target = Selector.GetTarget();
            if (TargetSelectorMenu.TargetLockItem)
            {
                if (!ComboMenu.ComboHotkeyItem || Target == null || !Target.IsValid || !Target.IsAlive)
                {
                    Target = target;
                }
            }
            else
            {
                Target = target;
            }
        }
    }
}
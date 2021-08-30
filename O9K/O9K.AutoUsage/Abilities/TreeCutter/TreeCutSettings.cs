﻿namespace O9K.AutoUsage.Abilities.TreeCutter
{
    using Core.Entities.Abilities.Base.Components.Base;
    using Core.Managers.Menu.Items;

    public class TreeCutSettings
    {
        public MenuSlider DelayBeforeNextActivation;

        public TreeCutSettings(Menu settings, IActiveAbility ability)
        {
            var menu = settings.GetOrAdd(new Menu(ability.DisplayName, ability.Name).SetTexture(ability.Name));

            this.DelayBeforeNextActivation =
                menu.Add(new MenuSlider("Time (in ms) before next activation (GLOBAL! WORK ON ALL CUTTERS)", 3000, 0,
                    5000));
        }

    }
}
namespace O9K.Hud.Modules.Screen.Time.AbilityHitTimings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    using Core.Entities.Abilities.Base;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using Divine;

    using Helpers;

    using MainMenu;

    using SharpDX;

    using AbilityEventArgs = Core.Managers.Menu.EventArgs.AbilityEventArgs;

    internal class AbilityHitTime : IHudModule
    {
        private readonly MenuSwitcher enabled;

        private readonly MenuHoldKey holdKey;

        private readonly MenuVectorSlider textPosition;

        private readonly MenuSlider textSize;

        private readonly List<AbilityTime> timings = new List<AbilityTime>();

        private readonly MenuToggleKey toggleKey;

        private readonly MenuAbilityToggler toggler;

        private UpdateHandler updateHandler;

        public AbilityHitTime(IHudMenu hudMenu)
        {
            var timeMenu = hudMenu.ScreenMenu.GetOrAdd(new Menu("Time"));
            timeMenu.AddTranslation(Lang.Ru, "Время");
            timeMenu.AddTranslation(Lang.Cn, "时间");

            var menu = timeMenu.Add(new Menu("Ability hit time"));
            menu.AddTranslation(Lang.Ru, "Время удара способности");
            menu.AddTranslation(Lang.Cn, "技能命中时间");

            this.enabled = menu.Add(new MenuSwitcher("Enabled", false).SetTooltip("Show required time for ability to reach mouse cursor"));
            this.enabled.AddTranslation(Lang.Ru, "Включено");
            this.enabled.AddTooltipTranslation(Lang.Ru, "Показать требуемое время для способности чтобы достичь курсор мыши");
            this.enabled.AddTranslation(Lang.Cn, "启用");
            this.enabled.AddTooltipTranslation(Lang.Cn, "显示到达鼠标光标所需的时间");

            var settings = menu.Add(new Menu("Settings"));
            settings.AddTranslation(Lang.Ru, "Настройки");
            settings.AddTranslation(Lang.Cn, "设置");

            this.textSize = settings.Add(new MenuSlider("Size", 17, 10, 35));
            this.textSize.AddTranslation(Lang.Ru, "Размер");
            this.textSize.AddTranslation(Lang.Cn, "大小");

            this.textPosition = new MenuVectorSlider(
                settings,
                new Vector3(34 * Hud.Info.ScreenRatio, -300, 300),
                new Vector3(40 * Hud.Info.ScreenRatio, -300, 300));

            var keys = menu.Add(new Menu("Keys"));
            keys.AddTranslation(Lang.Ru, "Клавишы");
            keys.AddTranslation(Lang.Cn, "键");

            this.toggleKey = keys.Add(new MenuToggleKey("Toggle key", Key.None, false)).SetTooltip("Show/hide timings");
            this.toggleKey.AddTranslation(Lang.Ru, "Клавиша переключения");
            this.toggleKey.AddTooltipTranslation(Lang.Ru, "Показать/скрыть тайминги");
            this.toggleKey.AddTranslation(Lang.Cn, "切换键");
            this.toggleKey.AddTooltipTranslation(Lang.Cn, "显示/隐藏时间");

            this.holdKey = keys.Add(new MenuHoldKey("Hold key", Key.LeftAlt)).SetTooltip("Show/hide timings");
            this.holdKey.AddTranslation(Lang.Ru, "Клавиша удержания");
            this.holdKey.AddTooltipTranslation(Lang.Ru, "Показать/скрыть тайминги");
            this.holdKey.AddTranslation(Lang.Cn, "按住键");
            this.holdKey.AddTooltipTranslation(Lang.Cn, "显示/隐藏时间");

            var abilitiesMenu = menu.Add(new Menu("Abilities"));
            abilitiesMenu.AddTranslation(Lang.Ru, "Способности");
            abilitiesMenu.AddTranslation(Lang.Cn, "播放声音");

            this.toggler = abilitiesMenu.Add(new MenuAbilityToggler("Enabled"));
            this.toggler.AddTranslation(Lang.Ru, "Включено");
            this.toggler.AddTranslation(Lang.Cn, "启用");
        }

        public void Activate()
        {
            this.updateHandler = UpdateManager.CreateIngameUpdate(0, false, this.OnUpdate);
            this.enabled.ValueChange += this.EnabledOnValueChange;
        }

        public void Dispose()
        {
            this.enabled.ValueChange -= this.EnabledOnValueChange;
            this.updateHandler.IsEnabled = false;
            RendererManager.Draw -= this.OnDraw;
            this.toggleKey.ValueChange -= this.KeyOnValueChange;
            this.holdKey.ValueChange -= this.HoldKeyOnValueChange;
            this.toggler.ValueChange -= this.TogglerOnValueChange;
            EntityManager9.AbilityAdded -= this.OnAbilityAdded;
            EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
            this.textPosition.Dispose();
            this.timings.Clear();
        }

        private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                this.toggleKey.ValueChange += this.KeyOnValueChange;
                EntityManager9.AbilityAdded += this.OnAbilityAdded;
                EntityManager9.AbilityRemoved += this.OnAbilityRemoved;
                this.toggler.ValueChange += this.TogglerOnValueChange;
                this.holdKey.ValueChange += this.HoldKeyOnValueChange;
            }
            else
            {
                this.toggleKey.ValueChange -= this.KeyOnValueChange;
                EntityManager9.AbilityAdded -= this.OnAbilityAdded;
                EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
                this.toggler.ValueChange -= this.TogglerOnValueChange;
                this.holdKey.ValueChange -= this.HoldKeyOnValueChange;
                this.updateHandler.IsEnabled = false;
                RendererManager.Draw -= this.OnDraw;
                this.timings.Clear();
            }
        }

        private void HoldKeyOnValueChange(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            this.toggleKey.IsActive = e.NewValue;
        }

        private void KeyOnValueChange(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            if (e.NewValue)
            {
                this.updateHandler.IsEnabled = true;
                RendererManager.Draw += this.OnDraw;
            }
            else
            {
                this.updateHandler.IsEnabled = false;
                RendererManager.Draw -= this.OnDraw;
            }
        }

        private void OnAbilityAdded(Ability9 ability)
        {
            try
            {
                if (!ability.Owner.IsMyHero || !(ability is PredictionAbility prediction))
                {
                    return;
                }

                this.toggler.AddAbility(prediction.Id, false);
                this.timings.Add(new AbilityTime(prediction));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnAbilityRemoved(Ability9 ability)
        {
            try
            {
                if (!ability.Owner.IsMyHero)
                {
                    return;
                }

                var find = this.timings.Find(x => x.Handle == ability.Handle);
                if (find != null)
                {
                    this.timings.Remove(find);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnDraw()
        {
            try
            {
                var startPosition = GameManager.MouseScreenPosition + this.textPosition;

                foreach (var ability in this.timings.Where(x => x.Display))
                {
                    var textureSize = new Vector2(this.textSize * Hud.Info.ScreenRatio * 1.75f);
                    var outlineSize = textureSize * 1.17f;
                    var outlinePosition = startPosition - new Vector2(
                                              (outlineSize.X - textureSize.X) / 2f,
                                              (outlineSize.Y - textureSize.Y) / 2f);

                    RendererManager.DrawTexture("o9k.outline", new RectangleF(outlinePosition.X, outlinePosition.Y, outlineSize.X, outlineSize.Y));
                    RendererManager.DrawTexture(ability.Name, new RectangleF(startPosition.X, startPosition.Y, textureSize.X, textureSize.Y), TextureType.RoundAbility);

                    RendererManager.DrawText(
                        ability.Time,
                        startPosition + new Vector2(textureSize.X * 1.2f, 0),
                        ability.Color,
                        this.textSize * Hud.Info.ScreenRatio);

                    startPosition += new Vector2(0, textureSize.Y * 1.2f);
                }
            }
            catch (InvalidOperationException)
            {
                // ignore
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnUpdate()
        {
            try
            {
                var position = GameManager.MousePosition;

                foreach (var ability in this.timings)
                {
                    if (!ability.Display)
                    {
                        continue;
                    }

                    ability.UpdateTime(position);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void TogglerOnValueChange(object sender, AbilityEventArgs e)
        {
            foreach (var ability in this.timings.Where(x => x.Name == e.Ability))
            {
                ability.Display = e.NewValue;
            }
        }
    }
}
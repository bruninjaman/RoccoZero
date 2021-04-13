namespace O9K.Hud.Modules.Map.Runes
{
    using System;
    using System.Linq;
    using System.Windows.Input;

    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.Items;

    using Divine;
    using Divine.SDK.Extensions;
    using Divine.SDK.Localization;

    using Helpers;

    using MainMenu;

    using SharpDX;

    internal class AllyBottle : IHudModule
    {
        private readonly MenuHoldKey holdKey;

        private readonly IMinimap minimap;

        public AllyBottle(IMinimap minimap, IHudMenu hudMenu)
        {
            this.minimap = minimap;

            var runesMenu = hudMenu.MapMenu.GetOrAdd(new Menu("Runes"));
            runesMenu.AddTranslation(Lang.Ru, "Руны");
            runesMenu.AddTranslation(Lang.Cn, "神符");

            var menu = runesMenu.Add(new Menu(LocalizationHelper.LocalizeName(AbilityId.item_bottle), "Bottle"));

            this.holdKey = menu.Add(new MenuHoldKey("Hold key", Key.LeftAlt)).SetTooltip("Show ally with bottle");
            this.holdKey.AddTranslation(Lang.Ru, "Клавиша удержания");
            this.holdKey.AddTooltipTranslation(Lang.Ru, "Показать союзников с " + LocalizationHelper.LocalizeName(AbilityId.item_bottle));
            this.holdKey.AddTranslation(Lang.Cn, "按住键");
            this.holdKey.AddTooltipTranslation(Lang.Cn, "显示与魔瓶的盟友");
        }

        public void Activate()
        {
            this.LoadTextures();

            this.holdKey.ValueChange += this.HoldKey_OnValueChange;
        }

        public void Dispose()
        {
            this.holdKey.ValueChange -= this.HoldKey_OnValueChange;
            RendererManager.Draw -= this.OnDraw;
        }

        private void HoldKey_OnValueChange(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            if (e.NewValue)
            {
                RendererManager.Draw += this.OnDraw;
            }
            else
            {
                RendererManager.Draw -= this.OnDraw;
            }
        }

        private void LoadTextures()
        {
            RendererManager.LoadTexture(
                "o9k.outline_hp",
                @"panorama\images\hud\reborn\buff_outline_psd.vtex_c",
                new TextureProperties
                {
                    ColorRatio = new Vector4(0f, 1f, 0f, 1f)
                });

            RendererManager.LoadTexture(
                "o9k.outline_mp",
                @"panorama\images\hud\reborn\buff_outline_psd.vtex_c",
                new TextureProperties
                {
                    ColorRatio = new Vector4(0f, 0.6f, 1f, 1f),
                    IsSliced = true
                });
        }

        private void OnDraw()
        {
            try
            {
                var rune = EntityManager.GetEntities<Rune>()
                    .Where(x => x.IsValid)
                    .OrderBy(x => x.Position.DistanceSquared(GameManager.MousePosition))
                    .FirstOrDefault();

                if (rune == null || !Hud.IsPositionOnScreen(rune.Position))
                {
                    return;
                }

                var allies = EntityManager9.Heroes.Where(x => !x.IsMyHero && !x.IsIllusion && x.IsAlly() && x.IsAlive);
                var textureSize = 45 * Hud.Info.ScreenRatio;
                var margin = 20 * Hud.Info.ScreenRatio;
                var position = this.minimap.WorldToScreen(rune.Position, textureSize);

                foreach (var hero in allies)
                {
                    var bottle = hero.Abilities.FirstOrDefault(x => x.Id == AbilityId.item_bottle);
                    if (bottle == null)
                    {
                        continue;
                    }

                    position.Y += textureSize + margin;

                    RendererManager.DrawTexture(hero.Name, position, TextureType.RoundUnit);

                    var outlinePosition = position * 1.25f;
                    RendererManager.DrawTexture("o9k.outline_hp", outlinePosition);
                    RendererManager.DrawTexture("o9k.outline_black" + (int)(100 - (hero.HealthPercentage / 2f)), outlinePosition);
                    RendererManager.DrawTexture("o9k.outline_mp" + (int)(hero.ManaPercentage / 2f), outlinePosition);

                    var chargesText = bottle.Charges.ToString("N0");
                    var chargesPosition = position.SinkToBottomRight(position.Width * 0.4f, position.Height * 0.4f);

                    RendererManager.DrawTexture("o9k.charge_bg", chargesPosition);
                    RendererManager.DrawTexture("o9k.outline_green", chargesPosition * 1.07f);
                    RendererManager.DrawText(
                        chargesText,
                        chargesPosition,
                        Color.White,
                        FontFlags.Center | FontFlags.VerticalCenter,
                        position.Width * 0.3f);
                }
            }
            catch (InvalidOperationException)
            {
                // ignored
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
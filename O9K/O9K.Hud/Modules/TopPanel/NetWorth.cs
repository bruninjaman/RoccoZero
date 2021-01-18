namespace O9K.Hud.Modules.TopPanel
{
    using System;
    using System.Collections.Generic;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Heroes.Unique;
    using Core.Entities.Units.Unique;
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

    internal class NetWorth : IHudModule
    {
        private readonly MenuSwitcher show;

        private readonly Dictionary<Team, int> teams = new Dictionary<Team, int>();

        private readonly ITopPanel topPanel;

        private Team ownerTeam;

        public NetWorth(ITopPanel topPanel, IHudMenu hudMenu)
        {
            this.topPanel = topPanel;

            var menu = hudMenu.TopPanelMenu.Add(new Menu("Net worth"));
            menu.AddTranslation(Lang.Ru, "Стоимость");
            menu.AddTranslation(Lang.Cn, "净值");

            this.show = menu.Add(new MenuSwitcher("Enabled", "enabled")).SetTooltip("Show net worth lead");
            this.show.AddTranslation(Lang.Ru, "Включено");
            this.show.AddTooltipTranslation(Lang.Ru, "Показывать разницу в стоимости предметов");
            this.show.AddTranslation(Lang.Cn, "启用");
            this.show.AddTooltipTranslation(Lang.Cn, "显示经济领先");
        }

        public void Activate()
        {
            this.LoadTextures();
            this.ownerTeam = EntityManager9.Owner.Team;

            this.show.ValueChange += this.ShowOnValueChange;
        }

        public void Dispose()
        {
            EntityManager9.AbilityAdded -= this.OnAbilityAdded;
            EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
            RendererManager.Draw -= this.OnDraw;
            this.show.ValueChange -= this.ShowOnValueChange;
        }

        private void LoadTextures()
        {
            RendererManager.LoadTexture(
                "o9k.net_worth_bg_top",
                @"panorama\images\masks\chat_preview_opacity_mask_png.vtex_c",
                new TextureProperties
                {
                    ColorRatio = new Vector4(0f, 0f, 0f, 0.6f)
                });
            RendererManager.LoadTexture(
                "o9k.net_worth_arrow_ally",
                @"panorama\images\hud\reborn\arrow_gold_dif_psd.vtex_c");
            RendererManager.LoadTexture(
                "o9k.net_worth_arrow_enemy",
                @"panorama\images\hud\reborn\arrow_plus_stats_red_psd.vtex_c");
        }

        private void OnAbilityAdded(Ability9 ability)
        {
            try
            {
                if (!ability.IsItem || ability.Owner.IsIllusion)
                {
                    return;
                }

                switch (ability.Owner)
                {
                    case Meepo meepo when !meepo.IsMainMeepo:
                        return;
                    case SpiritBear _:
                        break;
                    default:
                    {
                        if (!ability.Owner.IsHero)
                        {
                            return;
                        }

                        break;
                    }
                }

                this.teams[ability.Owner.Team] += (int)ability.BaseItem.Cost;
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
                if (!ability.IsItem || ability.Owner.IsIllusion)
                {
                    return;
                }

                switch (ability.Owner)
                {
                    case Meepo meepo when !meepo.IsMainMeepo:
                        return;
                    case SpiritBear _:
                        break;
                    default:
                    {
                        if (!ability.Owner.IsHero)
                        {
                            return;
                        }

                        break;
                    }
                }

                this.teams[ability.Owner.Team] -= (int)ability.BaseItem.Cost;
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
                var diff = this.teams[Team.Radiant] - this.teams[Team.Dire];

                string text;
                Team team;

                if (diff > 0)
                {
                    text = Math.Ceiling(diff / 1000f) + "k";
                    team = Team.Radiant;
                }
                else
                {
                    text = Math.Ceiling(diff / -1000f) + "k";
                    team = Team.Dire;
                }

                var ratio = Hud.Info.ScreenRatio;

                var position = this.topPanel.GetScorePosition(team);
                position.Y += position.Height + 1;
                position.Height = 22f * ratio;
                var textSize = 15 * ratio;
                var measureText = (position.Width - (RendererManager.MeasureText(text, textSize).X + (24 * ratio))) / 2f;

                RendererManager.DrawTexture("o9k.net_worth_bg_top", position);
                RendererManager.DrawTexture(
                    this.ownerTeam == team ? "o9k.net_worth_arrow_ally" : "o9k.net_worth_arrow_enemy",
                    new RectangleF(position.X + measureText, position.Y + (4 * ratio), 12 * ratio, 12 * ratio));

                RendererManager.DrawText(text, new Vector2(position.X + measureText + (15 * ratio), position.Y), Color.White, textSize);
            }
            catch (InvalidOperationException)
            {
                //ignore
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void ShowOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                this.teams[Team.Radiant] = 0;
                this.teams[Team.Dire] = 0;
                EntityManager9.AbilityAdded += this.OnAbilityAdded;
                EntityManager9.AbilityRemoved += this.OnAbilityRemoved;
                RendererManager.Draw += this.OnDraw;
            }
            else
            {
                EntityManager9.AbilityAdded -= this.OnAbilityAdded;
                EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
                RendererManager.Draw -= this.OnDraw;
            }
        }
    }
}
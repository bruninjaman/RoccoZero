﻿namespace O9K.Hud.Modules.Units.Abilities;

using System;
using System.Collections.Generic;
using System.Linq;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;
using Core.Helpers;
using Core.Logger;
using Core.Managers.Entity;
using Core.Managers.Menu;
using Core.Managers.Menu.EventArgs;
using Core.Managers.Menu.Items;
using Core.Managers.Renderer.Utils;

using Divine.Entity.Entities.Units.Components;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Renderer;

using Helpers;

using HudEntities.Units;

using MainMenu;

internal class Abilities : IHudModule
{
    private readonly MenuSwitcher abilitiesEnabled;

    private readonly MenuSwitcher abilitiesMinimalistic;

    private readonly MenuVectorSlider abilitiesPosition;

    private readonly MenuSwitcher abilitiesShowAlly;

    private readonly MenuSlider abilitiesSize;

    private readonly MenuSlider abilitiesTextSize;

    private readonly MenuSwitcher itemsEnabled;

    private readonly MenuVectorSlider itemsPosition;

    private readonly MenuSwitcher itemsShowAlly;

    private readonly MenuSlider itemsSize;

    private readonly MenuSlider itemsTextSize;

    private readonly MenuAbilityToggler itemsToggler;

    private readonly List<HudUnit> units = new List<HudUnit>();

    private readonly MenuSwitcher itemsShowMyHero;

    private readonly MenuSwitcher abilitiesShowMyHero;

    private readonly IHudMenu hudMenu;

    public Abilities(IHudMenu hudMenu)
    {
        this.hudMenu = hudMenu;

        var abilitiesMenu = hudMenu.UnitsMenu.Add(new Menu("Abilities"));
        abilitiesMenu.AddTranslation(Lang.Ru, "Способности");
        abilitiesMenu.AddTranslation(Lang.Cn, "播放声音");

        this.abilitiesEnabled = abilitiesMenu.Add(new MenuSwitcher("Enabled"));
        this.abilitiesEnabled.AddTranslation(Lang.Ru, "Включено");
        this.abilitiesEnabled.AddTranslation(Lang.Cn, "启用");

        this.abilitiesMinimalistic = abilitiesMenu.Add(new MenuSwitcher("Minimalistic mode", false));
        this.abilitiesMinimalistic.AddTranslation(Lang.Ru, "Минималистичный режим");
        this.abilitiesMinimalistic.AddTranslation(Lang.Cn, "简单模式");

        this.abilitiesShowAlly = abilitiesMenu.Add(new MenuSwitcher("Show ally abilities", false));
        this.abilitiesShowAlly.AddTranslation(Lang.Ru, "Показать способности союзников");
        this.abilitiesShowAlly.AddTranslation(Lang.Cn, "显示盟友技能");

        this.abilitiesShowMyHero = abilitiesMenu.Add(new MenuSwitcher("Show my abilities", false));
        this.abilitiesShowMyHero.AddTranslation(Lang.Ru, "Показать способности моего героя");
        this.abilitiesShowMyHero.AddTranslation(Lang.Cn, "展示我的能力");

        var abilitiesSettings = abilitiesMenu.Add(new Menu("Settings"));
        abilitiesSettings.AddTranslation(Lang.Ru, "Настройки");
        abilitiesSettings.AddTranslation(Lang.Cn, "设置");

        this.abilitiesPosition = new MenuVectorSlider(abilitiesSettings, new Vector3(0, -250, 250), new Vector3(0, -250, 250));
        this.abilitiesSize = abilitiesSettings.Add(new MenuSlider("Size", 25, 20, 50));
        this.abilitiesSize.AddTranslation(Lang.Ru, "Размер");
        this.abilitiesSize.AddTranslation(Lang.Cn, "大小");

        this.abilitiesTextSize = abilitiesSettings.Add(new MenuSlider("Cooldown size", 16, 10, 35));
        this.abilitiesTextSize.AddTranslation(Lang.Ru, "Размер текста");
        this.abilitiesTextSize.AddTranslation(Lang.Cn, "文本大小");

        var itemsMenu = hudMenu.UnitsMenu.Add(new Menu("Items"));
        itemsMenu.AddTranslation(Lang.Ru, "Предметы");
        itemsMenu.AddTranslation(Lang.Cn, "物品");

        this.itemsEnabled = itemsMenu.Add(new MenuSwitcher("Enabled"));
        this.itemsEnabled.AddTranslation(Lang.Ru, "Включено");
        this.itemsEnabled.AddTranslation(Lang.Cn, "启用");

        this.itemsShowAlly = itemsMenu.Add(new MenuSwitcher("Show ally items", false));
        this.itemsShowAlly.AddTranslation(Lang.Ru, "Показать предметы союзников");
        this.itemsShowAlly.AddTranslation(Lang.Cn, "盟友物品");

        this.itemsShowMyHero = itemsMenu.Add(new MenuSwitcher("Show my items", false));
        this.itemsShowMyHero.AddTranslation(Lang.Ru, "Показать предметы моего героя");
        this.itemsShowMyHero.AddTranslation(Lang.Cn, "显示我的物品");

        var itemsSettings = itemsMenu.Add(new Menu("Settings"));
        itemsSettings.AddTranslation(Lang.Ru, "Настройки");
        itemsSettings.AddTranslation(Lang.Cn, "设置");

        this.itemsPosition = new MenuVectorSlider(itemsSettings, new Vector3(0, -250, 250), new Vector3(0, -250, 250));
        this.itemsSize = itemsSettings.Add(new MenuSlider("Size", 25, 20, 50));
        this.itemsSize.AddTranslation(Lang.Ru, "Размер");
        this.itemsSize.AddTranslation(Lang.Cn, "大小");

        this.itemsTextSize = itemsSettings.Add(new MenuSlider("Cooldown size", 16, 10, 35));
        this.itemsTextSize.AddTranslation(Lang.Ru, "Размер текста");
        this.itemsTextSize.AddTranslation(Lang.Cn, "文本大小");

        var itemsDisplay = itemsMenu.Add(new Menu("Display"));
        itemsDisplay.AddTranslation(Lang.Ru, "Показывать");
        itemsDisplay.AddTranslation(Lang.Cn, "显示");

        this.itemsToggler = itemsDisplay.Add(new MenuAbilityToggler("Enabled"));
        this.itemsToggler.AddTranslation(Lang.Ru, "Включено");
        this.itemsToggler.AddTranslation(Lang.Cn, "启用");
    }

    public void Activate()
    {
        this.LoadTextures();

        EntityManager9.AbilityAdded += this.OnAbilityAdded;
        EntityManager9.AbilityRemoved += this.OnAbilityRemoved;
        EntityManager9.UnitRemoved += this.OnUnitRemoved;
        RendererManager.Draw += this.OnDraw;
        this.itemsToggler.ValueChange += this.ItemsTogglerOnValueChange;

        ////todo delete
        //if (AppDomain.CurrentDomain.GetAssemblies().Any(x => !x.GlobalAssemblyCache && x.GetName().Name.Contains("OverlayInformation")))
        //{
        //    Hud.DisplayWarning("O9K.Hud // OverlayInformation is already included in O9K.Hud");
        //}
    }

    public void Dispose()
    {
        EntityManager9.AbilityAdded -= this.OnAbilityAdded;
        EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
        EntityManager9.UnitRemoved -= this.OnUnitRemoved;
        RendererManager.Draw -= this.OnDraw;
        this.itemsToggler.ValueChange -= this.ItemsTogglerOnValueChange;
        this.abilitiesPosition.Dispose();
        this.itemsPosition.Dispose();
        this.units.Clear();
    }

    private float GetTextureScale(HudUnit unit)
    {
        var position = unit.Unit.Position;

        var closest = this.units.Where(x => x.Unit != unit.Unit && x.IsValid && x.Unit.DistanceSquared(position) < 50000)
            .OrderBy(x => x.Unit.DistanceSquared(position))
            .FirstOrDefault();

        if (closest != null)
        {
            var distance = position.DistanceSquared(closest.Unit.Position);

            return Math.Min(Math.Max(0.5f, distance / 50000f), 1f);
        }

        return 1;
    }

    private void ItemsTogglerOnValueChange(object sender, AbilityEventArgs e)
    {
        foreach (var ability in this.units.SelectMany(x => x.AllAbilities))
        {
            if (ability.Ability.Name == e.Ability)
            {
                ability.ChangeEnabled(e.NewValue);
            }
        }
    }

    private void LoadTextures()
    {
        RendererManager.LoadImage(
            "o9k.ability_level_rec",
            @"panorama\images\hud\reborn\levelup_button_3_psd.vtex_c",
            new ImageProperties
            {
                Brightness = 35
            });

        RendererManager.LoadImage(
            "o9k.ability_cd_bg",
            @"panorama\images\masks\softedge_horizontal_png.vtex_c",
            new ImageProperties
            {
                ColorTint = new Color(0, 0, 0, 153)
            });

        RendererManager.LoadImage(
            "o9k.ability_0lvl_bg",
            @"panorama\images\masks\softedge_horizontal_png.vtex_c",
            new ImageProperties
            {
                ColorTint = new Color(76, 76, 76, 102)
            });

        RendererManager.LoadImage(
            "o9k.ability_lvl_bg",
            @"panorama\images\masks\softedge_horizontal_png.vtex_c",
            new ImageProperties
            {
                ColorTint = new Color(0, 0, 0, 229)
            });

        RendererManager.LoadImage(
            "o9k.ability_minimal_bg",
            @"panorama\images\masks\softedge_horizontal_png.vtex_c",
            new ImageProperties
            {
                ColorTint = new Color(0, 0, 0, 178)
            });

        RendererManager.LoadImage(
            "o9k.ability_minimal_cd_bg",
            @"panorama\images\masks\softedge_horizontal_png.vtex_c",
            new ImageProperties
            {
                ColorTint = new Color(0, 0, 0)
            });

        RendererManager.LoadImage(
            "o9k.ability_minimal_mana_bg",
            @"panorama\images\masks\softedge_horizontal_png.vtex_c",
            new ImageProperties
            {
                ColorTint = new Color(0, 0, 229)
            });

        RendererManager.LoadImage(
            "o9k.ability_mana_bg",
            @"panorama\images\masks\softedge_horizontal_png.vtex_c",
            new ImageProperties
            {
                ColorTint = new Color(0, 0, 229, 204)
            });

        RendererManager.LoadImage(
            "o9k.charge_bg",
            @"panorama\images\masks\softedge_circle_sharper_png.vtex_c",
            new ImageProperties
            {
                ColorTint = new Color(0, 0, 0)
            });
    }

    private void OnAbilityAdded(Ability9 ability)
    {
        try
        {
            if (ability.IsTalent || ability.IsFake)
            {
                return;
            }

            var abilityOwner = ability.Owner;

            if (!abilityOwner.IsHero)
            {
                return;
            }

            if ((abilityOwner.UnitState & UnitState.CommandRestricted) != 0)
            {
                return;
            }

            var hudUnit = this.units.Find(x => x.Unit.Handle == abilityOwner.Handle);

            if (hudUnit == null)
            {
                hudUnit = new HudUnit(abilityOwner);
                this.units.Add(hudUnit);
            }

            hudUnit.AddAbility(ability, this.itemsToggler);
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
            if (ability.IsTalent || ability.IsFake)
            {
                return;
            }

            var abilityOwner = ability.Owner;

            if (!abilityOwner.IsHero)
            {
                return;
            }

            var unit = this.units.Find(x => x.Unit.Handle == abilityOwner.Handle);
            unit?.RemoveAbility(ability);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnDraw()
    {
        if (GameManager.IsShopOpen && this.hudMenu.DontDrawWhenShopIsOpen)
        {
            return;
        }

        try
        {
            foreach (var unit in this.units)
            {
                if (!unit.IsValid)
                {
                    continue;
                }

                var hpPosition = unit.HealthBarPosition;

                if (hpPosition.IsZero)
                {
                    continue;
                }

                var healthBarSize = unit.HealthBarSize;
                var scale = this.GetTextureScale(unit);

                if (this.abilitiesEnabled && (!unit.IsAlly || this.abilitiesShowAlly) && (!unit.Unit.IsMyHero || this.abilitiesShowMyHero))
                {
                    var abilities = unit.Abilities.ToArray();

                    if (this.abilitiesMinimalistic)
                    {
                        var start = (new Vector2(hpPosition.X + (healthBarSize.X * 0.5f), hpPosition.Y - this.abilitiesSize)
                                     + this.abilitiesPosition) - new Vector2((this.abilitiesSize * abilities.Length) / 2f, 0);

                        for (var i = 0; i < abilities.Length; i++)
                        {
                            abilities[i]
                                .DrawMinimalistic(
                                    new Rectangle9(
                                        start + new Vector2(i * this.abilitiesSize, 0),
                                        this.abilitiesSize,
                                        this.abilitiesSize * 0.35f),
                                    this.abilitiesTextSize);
                        }
                    }
                    else
                    {
                        var textureSize = this.abilitiesSize * scale;

                        var start = (new Vector2(hpPosition.X + (healthBarSize.X * 0.5f), hpPosition.Y - textureSize)
                                     + this.abilitiesPosition) - new Vector2((textureSize * abilities.Length) / 2f, 0);

                        for (var i = 0; i < abilities.Length; i++)
                        {
                            abilities[i]
                                .Draw(
                                    new Rectangle9(start + new Vector2(i * textureSize, 0), textureSize, textureSize),
                                    this.abilitiesTextSize * scale);
                        }
                    }
                }

                if (this.itemsEnabled && (!unit.IsAlly || this.itemsShowAlly) && (!unit.Unit.IsMyHero || this.itemsShowMyHero))
                {
                    var textureSize = this.itemsSize * scale;
                    var items = unit.Items.ToArray();

                    var start = (new Vector2(hpPosition.X + (healthBarSize.X * 0.5f), hpPosition.Y + 18) + this.itemsPosition)
                                - new Vector2((textureSize * items.Length) / 2f, 0);

                    for (var i = 0; i < items.Length; i++)
                    {
                        items[i]
                            .Draw(
                                new Rectangle9(start + new Vector2(i * textureSize, 0), textureSize, textureSize),
                                this.itemsTextSize * scale);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnUnitRemoved(Unit9 entity)
    {
        try
        {
            var hudUnit = this.units.Find(x => x.Unit.Handle == entity.Handle);

            if (hudUnit == null)
            {
                return;
            }

            this.units.Remove(hudUnit);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}
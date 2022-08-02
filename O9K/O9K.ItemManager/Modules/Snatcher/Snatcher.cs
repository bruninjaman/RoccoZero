﻿namespace O9K.ItemManager.Modules.Snatcher;

using System;
using System.Collections.Generic;
using System.Linq;

using Controllables;
using Controllables.Abilities;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;
using Core.Helpers;
using Core.Logger;
using Core.Managers.Entity;
using Core.Managers.Menu;
using Core.Managers.Menu.EventArgs;
using Core.Managers.Menu.Items;
using Divine.Entity;
using Divine.Game;
using Divine.Numerics;
using Divine.Order;
using Divine.Renderer;
using Divine.Update;
using Divine.Game.EventArgs;
using Divine.Order.EventArgs;
using Divine.Entity.Entities.PhysicalItems;
using Divine.Entity.Entities.Runes;
using Divine.Order.Orders.Components;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

using KeyEventArgs = Core.Managers.Menu.EventArgs.KeyEventArgs;
using Divine.Input;

internal class Snatcher : IModule
{
    private readonly MenuAbilityToggler aegisAbilityToggler;

    private readonly MenuAbilityToggler aegisDummyToggler;

    private readonly MenuHoldKey aegisKey;

    private readonly List<ControllableAbility> controllableAbilities = new List<ControllableAbility>();

    private readonly List<Controllable> controllables = new List<Controllable>();

    private readonly MenuSwitcher enabled;

    private readonly MenuHoldKey holdKey;

    private readonly MenuAbilityToggler holdToggler;

    private readonly HashSet<uint> ignoredItems = new HashSet<uint>();

    private readonly MultiSleeper itemSleeper = new MultiSleeper();

    private readonly Dictionary<AbilityId, bool> menuAbilities = new Dictionary<AbilityId, bool>
    {
        { AbilityId.ember_spirit_sleight_of_fist, true },
        { AbilityId.rattletrap_hookshot, true },
        { AbilityId.pangolier_swashbuckle, true },
        { AbilityId.sandking_burrowstrike, true },
        { AbilityId.item_blink, true },
    };

    private readonly Dictionary<string, AbilityId> menuItems = new Dictionary<string, AbilityId>
    {
        { "rune_doubledamage", AbilityId.dota_base_ability },
        { "item_aegis", AbilityId.item_aegis },
        { "item_rapier", AbilityId.item_rapier },
        { "item_cheese", AbilityId.item_cheese },
        { "item_refresher_shard", AbilityId.item_refresher_shard },
        { "item_ultimate_scepter_roshan", AbilityId.item_ultimate_scepter_roshan },
        { "item_aghanims_shard_roshan", AbilityId.item_aghanims_shard_roshan },
        { "item_gem", AbilityId.item_gem },
        { "item_seer_stone", AbilityId.item_seer_stone },
    };

    private readonly MenuSwitcher statusEnabled;

    private readonly MenuSlider statusX;

    private readonly MenuSlider statusY;

    private readonly MenuToggleKey toggleKey;

    private readonly MenuAbilityToggler toggleToggler;

    public Snatcher(IMainMenu mainMenu)
    {
        this.enabled = mainMenu.SnatcherMenu.Add(new MenuSwitcher("Enabled"));
        this.enabled.AddTranslation(Lang.Ru, "Включено");
        this.enabled.AddTranslation(Lang.Cn, "启用");

        var hold = mainMenu.SnatcherMenu.Add(new Menu("Hold"));
        hold.AddTranslation(Lang.Ru, "Удерживание");
        hold.AddTranslation(Lang.Cn, "按住");

        this.holdKey = hold.Add(new MenuHoldKey("Key"));
        this.holdKey.AddTranslation(Lang.Ru, "Клавиша");
        this.holdKey.AddTranslation(Lang.Cn, "键");

        this.holdToggler = hold.Add(new MenuAbilityToggler("Take"));
        this.holdToggler.AddTranslation(Lang.Ru, "Забирать");
        this.holdToggler.AddTranslation(Lang.Cn, "拿起");

        this.aegisKey = hold.Add(new MenuHoldKey("Aegis key").SetTooltip("Steal aegis with abilities"));
        this.aegisKey.AddTranslation(Lang.Ru, "Клавиша аегиса");
        this.aegisKey.AddTooltipTranslation(Lang.Ru, "Украсть аегис используя способности");
        this.aegisKey.AddTranslation(Lang.Cn, "不朽盾键位");
        this.aegisKey.AddTooltipTranslation(Lang.Cn, "用技能偷盾");

        this.aegisAbilityToggler = hold.Add(new MenuAbilityToggler("Abilities", this.menuAbilities));
        this.aegisAbilityToggler.AddTranslation(Lang.Ru, "Способности");
        this.aegisAbilityToggler.AddTranslation(Lang.Cn, "技能");

        var toggle = mainMenu.SnatcherMenu.Add(new Menu("Toggle"));
        toggle.AddTranslation(Lang.Ru, "Переключение");
        toggle.AddTranslation(Lang.Cn, "切换");

        this.toggleKey = toggle.Add(new MenuToggleKey("Key", Key.None, false));
        this.toggleKey.AddTranslation(Lang.Ru, "Клавиша");
        this.toggleKey.AddTranslation(Lang.Cn, "键");

        this.toggleToggler = toggle.Add(new MenuAbilityToggler("Take"));
        this.toggleToggler.AddTranslation(Lang.Ru, "Забирать");
        this.toggleToggler.AddTranslation(Lang.Cn, "拿起");

        var status = mainMenu.SnatcherMenu.Add(new Menu("Status"));
        status.AddTranslation(Lang.Ru, "Статус");
        status.AddTranslation(Lang.Cn, "状态");

        this.statusEnabled = status.Add(new MenuSwitcher("Enabled", false).SetTooltip("Show when snatcher is active"));
        this.statusEnabled.AddTranslation(Lang.Ru, "Включено");
        this.statusEnabled.AddTooltipTranslation(Lang.Ru, "Показывать когда снатчер включен");
        this.statusEnabled.AddTranslation(Lang.Cn, "启用");
        this.statusEnabled.AddTooltipTranslation(Lang.Cn, "显示神符抢夺者状态");

        this.statusX = status.Add(new MenuSlider("Position X", (int)(Hud.Info.ScreenSize.X * 0.01f), 0, (int)Hud.Info.ScreenSize.X));
        this.statusX.AddTranslation(Lang.Ru, "X позиция");
        this.statusX.AddTranslation(Lang.Cn, "X位置");

        this.statusY = status.Add(new MenuSlider("Position Y", (int)(Hud.Info.ScreenSize.Y * 0.1f), 0, (int)Hud.Info.ScreenSize.Y));
        this.statusY.AddTranslation(Lang.Ru, "Y позиция");
        this.statusY.AddTranslation(Lang.Cn, "Y位置");

        // dummy
        this.aegisDummyToggler = new MenuAbilityToggler("dummy");
    }

    public void Activate()
    {
        this.LoadTextures();

        this.enabled.ValueChange += this.EnabledOnValueChange;
    }

    public void Dispose()
    {
        this.enabled.ValueChange -= this.EnabledOnValueChange;
        EntityManager9.UnitAdded -= this.OnUnitAdded;
        EntityManager9.UnitRemoved -= this.OnUnitRemoved;
        EntityManager9.AbilityAdded -= this.OnAbilityAdded;
        EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
        this.statusEnabled.ValueChange -= this.StatusEnabledOnValueChange;
        this.holdKey.ValueChange -= this.HoldKeyOnValueChange;
        UpdateManager.Update -= this.HoldOnUpdate;
        OrderManager.OrderAdding -= this.OnOrderAdding;
        this.toggleKey.ValueChange -= this.ToggleKeyOnValueChange;
        this.aegisKey.ValueChange -= this.AegisKeyOnValueChange;
        GameManager.GameEvent -= this.OnGameEvent;
        UpdateManager.Update -= this.AegisOnUpdate;
        RendererManager.Draw -= this.OnDraw;
    }

    private void AegisKeyOnValueChange(object sender, KeyEventArgs e)
    {
        if (e.NewValue)
        {
            GameManager.GameEvent += this.OnGameEvent;
            UpdateManager.Update += this.AegisOnUpdate;
        }
        else
        {
            GameManager.GameEvent -= this.OnGameEvent;
            UpdateManager.Update -= this.AegisOnUpdate;
        }
    }

    private void AegisOnUpdate()
    {
        try
        {
            this.PickOnUpdate(this.aegisDummyToggler);

            var roshan = EntityManager9.Units.FirstOrDefault(x => x.IsVisible && x.IsAlive && x.Name == "npc_dota_roshan");
            if (roshan == null)
            {
                return;
            }

            this.AegisSteal(roshan);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void AegisSteal(Unit9 roshan)
    {
        if (GameManager.IsPaused)
        {
            return;
        }

        var ability = this.controllableAbilities.Find(x => this.aegisAbilityToggler.IsEnabled(x.Ability.Name) && x.CanBeCasted);
        ability?.UseAbility(roshan);
    }

    private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            EntityManager9.UnitAdded += this.OnUnitAdded;
            EntityManager9.UnitRemoved += this.OnUnitRemoved;
            EntityManager9.AbilityAdded += this.OnAbilityAdded;
            EntityManager9.AbilityRemoved += this.OnAbilityRemoved;
            OrderManager.OrderAdding += this.OnOrderAdding;
            this.statusEnabled.ValueChange += this.StatusEnabledOnValueChange;
            this.holdKey.ValueChange += this.HoldKeyOnValueChange;
            this.toggleKey.ValueChange += this.ToggleKeyOnValueChange;
            this.aegisKey.ValueChange += this.AegisKeyOnValueChange;
        }
        else
        {
            EntityManager9.UnitAdded -= this.OnUnitAdded;
            EntityManager9.UnitRemoved -= this.OnUnitRemoved;
            EntityManager9.AbilityAdded -= this.OnAbilityAdded;
            EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
            OrderManager.OrderAdding -= this.OnOrderAdding;
            this.statusEnabled.ValueChange -= this.StatusEnabledOnValueChange;
            this.holdKey.ValueChange -= this.HoldKeyOnValueChange;
            UpdateManager.Update -= this.HoldOnUpdate;
            this.toggleKey.ValueChange -= this.ToggleKeyOnValueChange;
            UpdateManager.DestroyIngameUpdate(this.ToggleOnUpdate);
            this.aegisKey.ValueChange -= this.AegisKeyOnValueChange;
            GameManager.GameEvent -= this.OnGameEvent;
            UpdateManager.Update -= this.AegisOnUpdate;
            RendererManager.Draw -= this.OnDraw;
        }
    }

    private void HoldKeyOnValueChange(object sender, KeyEventArgs e)
    {
        if (e.NewValue)
        {
            UpdateManager.Update += this.HoldOnUpdate;
        }
        else
        {
            UpdateManager.Update -= this.HoldOnUpdate;
        }
    }

    private void HoldOnUpdate()
    {
        try
        {
            this.PickOnUpdate(this.holdToggler);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private bool IsItemEnabled(MenuAbilityToggler toggler, PhysicalItem item)
    {
        if (item.Item.NeutralTierIndex >= 0 && toggler.IsEnabled("item_seer_stone"))
        {
            return true;
        }

        if (toggler.IsEnabled(item.Item.Name))
        {
            return true;
        }

        return false;
    }

    private void LoadTextures()
    {
        foreach (var ability in this.menuAbilities)
        {
            RendererManager.LoadImage(ability.Key);
        }

        RendererManager.LoadImage(
            "rune_doubledamage",
            @"panorama\images\spellicons\rune_doubledamage_png.vtex_c");

        foreach (var ability in this.menuItems)
        {
            if (ability.Value == AbilityId.dota_base_ability)
            {
                continue;
            }

            RendererManager.LoadImage(ability.Value);
        }

        foreach (var ability in this.menuItems)
        {
            this.holdToggler.AddAbility(ability.Key);
            this.toggleToggler.AddAbility(ability.Key);
        }

        this.aegisDummyToggler.AddAbility(AbilityId.item_aegis);
        this.aegisDummyToggler.AddAbility(AbilityId.item_cheese);
        this.aegisDummyToggler.AddAbility(AbilityId.item_refresher_shard);
        this.aegisDummyToggler.AddAbility(AbilityId.item_ultimate_scepter_roshan);
        this.aegisDummyToggler.AddAbility(AbilityId.item_aghanims_shard_roshan);
    }

    private void OnAbilityAdded(Ability9 ability)
    {
        try
        {
            if (!this.menuAbilities.ContainsKey(ability.Id) || !ability.Owner.CanUseAbilities || !ability.Owner.IsMyControllable)
            {
                return;
            }

            switch (ability.Id)
            {
                case AbilityId.ember_spirit_sleight_of_fist:
                {
                    this.controllableAbilities.Add(new SleightOfFist((ActiveAbility)ability));
                    break;
                }
                case AbilityId.pangolier_swashbuckle:
                {
                    this.controllableAbilities.Add(new Swashbuckle((ActiveAbility)ability));
                    break;
                }
                default:
                {
                    this.controllableAbilities.Add(new ControllableAbility((ActiveAbility)ability));
                    break;
                }
            }
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
            if (!this.menuAbilities.ContainsKey(ability.Id) || !ability.Owner.CanUseAbilities || !ability.Owner.IsMyControllable)
            {
                return;
            }

            var remove = this.controllableAbilities.Find(x => x.Handle == ability.Handle);
            if (remove != null)
            {
                this.controllableAbilities.Remove(remove);
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
            var show = false;
            var text = "Snatcher ";

            if (this.toggleKey)
            {
                text += "(T)";
                show = true;
            }

            if (this.holdKey)
            {
                text += "(H)";
                show = true;
            }

            if (this.aegisKey)
            {
                text += "(A)";
                show = true;
            }

            if (!show)
            {
                return;
            }

            RendererManager.DrawText(text, new Vector2(this.statusX, this.statusY), Color.LawnGreen, 20 * Hud.Info.ScreenRatio);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnOrderAdding(OrderAddingEventArgs e)
    {
        try
        {
            if (e.IsCustom || !e.Process)
            {
                return;
            }

            var order = e.Order;
            if (order.Type == OrderType.DropItem)
            {
                this.ignoredItems.Add(order.Ability.Handle);
            }
            else if (order.Type == OrderType.PickUpItem)
            {
                var physicalItem = order.Target as PhysicalItem;
                if (physicalItem != null)
                {
                    this.ignoredItems.Remove(physicalItem.Item.Handle);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    private void OnGameEvent(GameEventEventArgs e)
    {
        if (e.GameEvent.Name != "dota_roshan_kill")
        {
            return;
        }

        try
        {
            this.AegisSteal(null);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    private void OnUnitAdded(Unit9 unit)
    {
        try
        {
            if (!unit.IsMyControllable)
            {
                return;
            }

            if (unit.IsMyHero)
            {
                this.controllables.Add(new Controllable(unit));
                return;
            }

            switch (unit.Name)
            {
                case "npc_dota_lone_druid_bear1":
                case "npc_dota_lone_druid_bear2":
                case "npc_dota_lone_druid_bear3":
                case "npc_dota_lone_druid_bear4":
                {
                    this.controllables.Add(new SpiritBear(unit));
                    break;
                }
                case "npc_dota_hero_meepo":
                {
                    this.controllables.Add(new MeepoClone(unit));
                    break;
                }
                case "npc_dota_hero_arc_warden":
                {
                    this.controllables.Add(new ArcWardenClone(unit));
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnUnitRemoved(Unit9 unit)
    {
        try
        {
            if (!unit.IsMyControllable)
            {
                return;
            }

            var remove = this.controllables.Find(x => x.Handle == unit.Handle);
            if (remove != null)
            {
                this.controllables.Remove(remove);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void PickOnUpdate(MenuAbilityToggler toggler)
    {
        if (GameManager.IsPaused)
        {
            return;
        }

        var validControllables = this.controllables.Where(x => x.IsValid).ToList();

        if (toggler.IsEnabled("rune_doubledamage"))
        {
            var runes = EntityManager.GetEntities<Rune>().Where(x => !this.itemSleeper.IsSleeping(x.Handle));

            foreach (var rune in runes)
            {
                foreach (var controllable in validControllables)
                {
                    if (controllable.CanPick(rune))
                    {
                        controllable.Pick(rune);
                        this.itemSleeper.Sleep(rune.Handle, 0.5f);
                        return;
                    }
                }
            }
        }

        var items = EntityManager.GetEntities<PhysicalItem>()
            .Where(
                x => !this.ignoredItems.Contains(x.Item.Handle) && !this.itemSleeper.IsSleeping(x.Handle)
                                                                && this.IsItemEnabled(toggler, x))
            .OrderByDescending(x => x.Item.Id == AbilityId.item_aegis);

        foreach (var item in items)
        {
            foreach (var controllable in validControllables)
            {
                if (controllable.CanPick(item))
                {
                    controllable.Pick(item);
                    this.itemSleeper.Sleep(item.Handle, 0.5f);
                    return;
                }
            }
        }
    }

    private void StatusEnabledOnValueChange(object sender, SwitcherEventArgs e)
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

    private void ToggleKeyOnValueChange(object sender, KeyEventArgs e)
    {
        if (e.NewValue)
        {
            UpdateManager.CreateIngameUpdate(300, this.ToggleOnUpdate);
        }
        else
        {
            UpdateManager.DestroyIngameUpdate(this.ToggleOnUpdate);
        }
    }

    private void ToggleOnUpdate()
    {
        try
        {
            this.PickOnUpdate(this.toggleToggler);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}
﻿namespace O9K.ItemManager.Modules.AutoActions;

using System;
using System.Collections.Generic;
using System.Linq;

using Core.Entities.Abilities.Base;
using Core.Entities.Heroes;
using Core.Entities.Units;
using Core.Logger;
using Core.Managers.Entity;
using Core.Managers.Menu;
using Core.Managers.Menu.EventArgs;
using Core.Managers.Menu.Items;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Abilities.Items.Components;
using Divine.Entity.Entities.Players;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Helpers;
using Divine.Numerics;
using Divine.Order;
using Divine.Order.EventArgs;
using Divine.Order.Orders.Components;
using Divine.Update;

using Metadata;

internal class ItemSwap : IModule
{
    private readonly MenuSwitcher consumableItems;

    private readonly HashSet<AbilityId> items = new HashSet<AbilityId>
    {
        AbilityId.item_aegis,
        AbilityId.item_cheese,
        AbilityId.item_moon_shard,
        AbilityId.item_refresher_shard,
        AbilityId.item_ultimate_scepter,
        AbilityId.item_enchanted_mango,
        AbilityId.item_clarity,
        AbilityId.item_flask,
        AbilityId.item_tango,
        AbilityId.item_tango_single,
        AbilityId.item_ward_observer,
        AbilityId.item_ward_sentry,
        AbilityId.item_dust,
        AbilityId.item_tome_of_knowledge,
        AbilityId.item_smoke_of_deceit,
        AbilityId.item_branches,
        AbilityId.item_infused_raindrop,
        AbilityId.item_faerie_fire
    };

    private readonly HashSet<AbilityId> itemsNeutral = new HashSet<AbilityId>
    {
        AbilityId.item_mango_tree,
        AbilityId.item_royal_jelly,
        AbilityId.item_repair_kit,
        AbilityId.item_greater_faerie_fire
    };

    private readonly MenuSwitcher neutralItems;

    private readonly MenuSwitcher philosophersStone;

    private bool moveStoneToStash;

    private Owner owner;

    private Item swapBackItem;

    public ItemSwap(IMainMenu mainMenu)
    {
        var menu = mainMenu.AutoActionsMenu.Add(new Menu("Item auto swap"));
        menu.AddTranslation(Lang.Ru, "Автоматическое перекладывание предметов");
        menu.AddTranslation(Lang.Cn, "物品的自动转移");

        this.consumableItems = menu.Add(new MenuSwitcher("Consumable items"))
            .SetTooltip("Take items from backpack when consumable used");
        this.consumableItems.AddTranslation(Lang.Ru, "Расходуемые предметы");
        this.consumableItems.AddTooltipTranslation(Lang.Ru, "Брать вещи из рюкзака при использовании расходуемых предметов");
        this.consumableItems.AddTranslation(Lang.Cn, "消耗品");
        this.consumableItems.AddTooltipTranslation(Lang.Cn, "使用消耗品时从背包中取出物品");

        this.neutralItems = menu.Add(new MenuSwitcher("Neutral items"))
            .SetTooltip("Take neutral items from backpack when used/transferred");
        this.neutralItems.AddTranslation(Lang.Ru, "Нейтральные предметы");
        this.consumableItems.AddTooltipTranslation(Lang.Ru, "Брать вещи из рюкзака при использовании/передаче");
        this.neutralItems.AddTranslation(Lang.Cn, "中立物品");
        this.consumableItems.AddTooltipTranslation(Lang.Cn, "使用/转移时从背包中拿东西");

        this.philosophersStone = menu.Add(new MenuSwitcher(LocalizationHelper.LocalizeName(AbilityId.item_philosophers_stone), "stone"))
            .SetTooltip("Take item on death");
        this.philosophersStone.AddTooltipTranslation(Lang.Ru, "Взять предмет при смерти");
        this.philosophersStone.AddTooltipTranslation(Lang.Cn, "死亡时取物品");
    }

    public void Activate()
    {
        this.owner = EntityManager9.Owner;
        this.consumableItems.ValueChange += this.ConsumableItemsOnValueChange;
        this.neutralItems.ValueChange += this.NeutralItemsOnValueChange;
        this.philosophersStone.ValueChange += this.PhilosophersStoneOnValueChange;
    }

    public void Dispose()
    {
        this.neutralItems.ValueChange -= this.NeutralItemsOnValueChange;
        this.consumableItems.ValueChange -= this.ConsumableItemsOnValueChange;
        this.philosophersStone.ValueChange -= this.PhilosophersStoneOnValueChange;
        EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
        EntityManager9.AbilityRemoved -= this.OnNeutralAbilityRemoved;
        OrderManager.OrderAdding -= OnOrderAdding;
        EntityManager9.UnitMonitor.UnitDied -= this.UnitMonitorOnUnitDied;
    }

    private void ConsumableItemsOnValueChange(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            EntityManager9.AbilityRemoved += this.OnAbilityRemoved;
        }
        else
        {
            EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
        }
    }

    private void MoveItemToNeutralStash(Item item)
    {
        try
        {
            if (!this.moveStoneToStash)
            {
                return;
            }

            this.moveStoneToStash = false;
            OrderManager.CreateOrder(OrderType.DropItemAtFountain, new[] { this.owner.Hero.BaseHero }, 0, item.Index, Vector3.Zero, false, false, false);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void NeutralItemsOnValueChange(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            OrderManager.OrderAdding += OnOrderAdding;
            EntityManager9.AbilityRemoved += this.OnNeutralAbilityRemoved;
        }
        else
        {
            OrderManager.OrderAdding -= OnOrderAdding;
            EntityManager9.AbilityRemoved -= this.OnNeutralAbilityRemoved;
        }
    }

    private void OnAbilityRemoved(Ability9 ability)
    {
        try
        {
            if (ability.Owner != this.owner.Hero || !this.items.Contains(ability.Id))
            {
                return;
            }

            UpdateManager.BeginInvoke(545, this.SwapItem);
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
            if (e.IsCustom || e.Order.Type != OrderType.DropItemAtFountain)
            {
                return;
            }

            UpdateManager.BeginInvoke(589, this.SwapNeutralItem);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    private void OnNeutralAbilityRemoved(Ability9 ability)
    {
        try
        {
            if (ability.Owner != this.owner.Hero || !this.itemsNeutral.Contains(ability.Id))
            {
                return;
            }

            UpdateManager.BeginInvoke(511, this.SwapNeutralItem);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnUpdateStone()
    {
        try
        {
            var hero = this.owner.Hero;
            if (hero?.IsAlive != true)
            {
                return;
            }

            var stone = hero.BaseInventory.NeutralItem;
            if (stone?.Id != AbilityId.item_philosophers_stone)
            {
                this.swapBackItem = null;
                this.moveStoneToStash = false;
                UpdateManager.DestroyIngameUpdate(this.OnUpdateStone);
                return;
            }

            UpdateManager.BeginInvoke(125, () => this.MoveItemToNeutralStash(stone));
            UpdateManager.BeginInvoke(355, this.SwapOldNeutralItem);

            UpdateManager.DestroyIngameUpdate(this.OnUpdateStone);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void PhilosophersStoneOnValueChange(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            EntityManager9.UnitMonitor.UnitDied += this.UnitMonitorOnUnitDied;
        }
        else
        {
            EntityManager9.UnitMonitor.UnitDied -= this.UnitMonitorOnUnitDied;
        }
    }

    private void SwapItem()
    {
        try
        {
            var inventory = this.owner.Hero.BaseHero.Inventory;
            if (!inventory.FreeMainSlots.Any())
            {
                return;
            }

            var backPackItem = inventory.BackpackItems.Where(x => x.NeutralTierIndex < 0 && !x.IsRecipe)
                .OrderByDescending(x => x.Cost)
                .FirstOrDefault();

            if (backPackItem == null)
            {
                return;
            }

            backPackItem.Move(inventory.FreeMainSlots.First());
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void SwapNeutralItem()
    {
        try
        {
            var neutralItemInSlot = this.owner.Hero.BaseHero.Inventory.GetItem(ItemSlot.NeutralItemSlot);
            if (neutralItemInSlot != null)
            {
                return;
            }

            var neutralItem = this.owner.Hero.BaseHero.Inventory.BackpackItems.FirstOrDefault(x => x.NeutralTierIndex >= 0);
            if (neutralItem == null)
            {
                return;
            }

            neutralItem.Move(ItemSlot.NeutralItemSlot);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void SwapOldNeutralItem()
    {
        try
        {
            if (this.swapBackItem == null)
            {
                return;
            }

            var inventory = this.owner.Hero.BaseInventory;

            for (var i = ItemSlot.BackPack1; i <= ItemSlot.StashSlot6; i++)
            {
                var slotItem = inventory.GetItem(i);
                if (slotItem?.Handle == this.swapBackItem.Handle)
                {
                    Player.Move(this.owner.Hero, this.swapBackItem, ItemSlot.NeutralItemSlot);
                    break;
                }
            }

            this.swapBackItem = null;
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void TakeItemFromBackpack(Item stone)
    {
        try
        {
            var inventory = this.owner.Hero.BaseInventory;
            var neutralItem = inventory.NeutralItem;

            if (neutralItem?.Handle != stone.Handle)
            {
                this.swapBackItem = neutralItem;
                Player.Move(this.owner.Hero, stone, ItemSlot.NeutralItemSlot);
            }

            UpdateManager.CreateIngameUpdate(100, this.OnUpdateStone);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void TakeItemFromStash(Item stone)
    {
        try
        {
            var inventory = this.owner.Hero.BaseInventory;
            var neutralItem = inventory.GetItem(ItemSlot.NeutralItemSlot);
            var freeSlots = inventory.FreeBackpackSlots.Concat(inventory.FreeStashSlots);
            if (!freeSlots.Any() && neutralItem != null)
            {
                return;
            }

            this.moveStoneToStash = true;

            OrderManager.CreateOrder(OrderType.TakeItemFromNeutralItemStash, new[] { this.owner.Hero.BaseHero }, 0, stone.Index, Vector3.Zero, false, false, false);

            UpdateManager.BeginInvoke(500, () => this.TakeItemFromBackpack(stone));
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void UnitMonitorOnUnitDied(Unit9 unit)
    {
        try
        {
            if (!unit.IsMyHero || unit.BaseInventory.NeutralItem?.Id == AbilityId.item_philosophers_stone)
            {
                return;
            }

            var stone = EntityManager.GetEntities<Item>()
                .FirstOrDefault(x => x.Id == AbilityId.item_philosophers_stone && x.Team == this.owner.Team);

            if (stone == null || !(stone.Owner is Hero stoneOwner))
            {
                return;
            }

            for (var i = ItemSlot.BackPack1; i <= ItemSlot.NeutralItemSlot; i++)
            {
                var slotItem = stoneOwner.Inventory.GetItem(i);
                if (slotItem?.Handle == stone.Handle)
                {
                    if (stoneOwner.Handle == unit.Handle)
                    {
                        UpdateManager.BeginInvoke(500, () => this.TakeItemFromBackpack(stone));
                    }

                    return;
                }
            }

            UpdateManager.BeginInvoke(500, () => this.TakeItemFromStash(stone));
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}
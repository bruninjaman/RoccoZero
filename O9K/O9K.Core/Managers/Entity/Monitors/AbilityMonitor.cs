namespace O9K.Core.Managers.Entity.Monitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Divine;

    using Entities.Abilities.Base;
    using Entities.Abilities.Base.Components;
    using Entities.Units;

    using Logger;

    public sealed class AbilityMonitor : IDisposable
    {
        private readonly HashSet<string> canUseOwnerItems = new HashSet<string>
        {
            "npc_dota_lone_druid_bear1",
            "npc_dota_lone_druid_bear2",
            "npc_dota_lone_druid_bear3",
            "npc_dota_lone_druid_bear4"
        };

        private readonly ItemSlot[] inventoryItemSlots =
        {
            ItemSlot.MainSlot_1,
            ItemSlot.MainSlot_2,
            ItemSlot.MainSlot_3,
            ItemSlot.MainSlot_4,
            ItemSlot.MainSlot_5,
            ItemSlot.MainSlot_6,
            ItemSlot.TownPortalScrollSlot,
            ItemSlot.NeutralItemSlot
        };

        private readonly ItemSlot[] stashItemSlots =
        {
            ItemSlot.BackPack_1,
            ItemSlot.BackPack_2,
            ItemSlot.BackPack_3,
            ItemSlot.StashSlot_1,
            ItemSlot.StashSlot_2,
            ItemSlot.StashSlot_3,
            ItemSlot.StashSlot_4,
            ItemSlot.StashSlot_5,
            ItemSlot.StashSlot_6
        };

        public AbilityMonitor()
        {
            Entity.NetworkPropertyChanged += OnNetworkPropertyChanged;
            EntityManager.EntityAdded += OnEntityAdded;
            EntityManager.EntityRemoved += OnEntityRemoved;

            //UpdateManager.Subscribe(this.GameOnUpdate2, 500);
            //GameManager.Update += this.GameOnUpdate;
        }

        /*private void GameOnUpdate2()
        {
            //hack
            try
            {
                foreach (var unit in EntityManager9.Units.Where(x => x.IsVisible))
                {
                    var inventory = unit.BaseUnit?.Inventory;
                    if (inventory == null)
                    {
                        continue;
                    }

                    var checkedItems = new List<uint>();

                    foreach (var handle in this.GetInventoryItems(inventory).Select(x => x.Handle))
                    {
                        var item = EntityManager9.GetAbilityFast(handle);
                        if (item == null)
                        {
                            continue;
                        }

                        checkedItems.Add(handle);

                        if (item.Owner == unit)
                        {
                            item.IsAvailable = true;
                            continue;
                        }

                        EntityManager9.RemoveAbility(item.BaseAbility);
                        EntityManager9.AddAbility(item.BaseAbility);
                    }

                    
                    foreach (var handle in this.GetStashItems(inventory).Select(x => x.Handle))
                    {
                        var item = EntityManager9.GetAbilityFast(handle);
                        if (item == null)
                        {
                            continue;
                        }

                        checkedItems.Add(handle);

                        if (item.Owner == unit)
                        {
                            item.IsAvailable = false;
                            continue;
                        }

                        EntityManager9.RemoveAbility(item.BaseAbility);
                        EntityManager9.AddAbility(item.BaseAbility);
                    }

                    // stashed neutral items
                    foreach (var item in unit.AbilitiesFast.Where(
                        x => x.IsItem && x.IsAvailable && !checkedItems.Contains(x.Handle)))
                    {
                        item.IsAvailable = false;
                    }

                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private readonly HashSet<uint> abilitiesInPhase = new HashSet<uint>();

        private void GameOnUpdate(EventArgs args)
        {
            //hack
            try
            {
                foreach (var ability in EntityManager9.Abilities.OfType<ActiveAbility>())
                {
                    if (!ability.Owner.IsAlive || !ability.Owner.IsVisible)
                    {
                        continue;
                    }

                    if (ability.BaseAbility.IsInAbilityPhase)
                    {
                        if (this.abilitiesInPhase.Add(ability.Handle))
                        {
                            ability.Owner.IsCasting = ability.IsCasting = true;
                            this.AbilityCastChange?.Invoke(ability);
                        }
                    }
                    else
                    {
                        if (this.abilitiesInPhase.Remove(ability.Handle))
                        {
                            ability.Owner.IsCasting = ability.IsCasting = false;
                            this.AbilityCastChange?.Invoke(ability);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }*/

        public delegate void EventHandler(Ability9 ability);

        public event EventHandler AbilityCastChange;

        public event EventHandler AbilityCasted;

        public event EventHandler AbilityChannel;

        public event System.EventHandler InventoryChanged;

        public void Dispose()
        {
            Entity.NetworkPropertyChanged += OnNetworkPropertyChanged;
            EntityManager.EntityAdded -= OnEntityAdded;
            EntityManager.EntityRemoved -= OnEntityRemoved;
        }

        internal void SetOwner(Ability9 ability, Unit9 owner)
        {
            if (!ability.IsItem)
            {
                ability.SetOwner(owner);
                return;
            }

            var item = ability.BaseItem;
            var itemPurchaser = item.Purchaser?.Hero;

            if (item.Shareability == Shareability.Full || owner.IsIllusion || itemPurchaser?.IsValid != true || owner.Owner?.IsValid != true
                || (this.canUseOwnerItems.Contains(owner.Name) && itemPurchaser.Handle == owner.Owner.Handle))
            {
                ability.SetOwner(owner);
            }
            else
            {
                ability.SetOwner(EntityManager9.AddUnit(itemPurchaser));
            }

            this.UpdateItemState(ability);
        }

        private void OnEntityAdded(EntityAddedEventArgs e)
        {
            try
            {
                var physicalItem = e.Entity as PhysicalItem;
                if (physicalItem == null || physicalItem.Item == null)
                {
                    return;
                }

                var item = EntityManager9.GetAbilityFast(physicalItem.Item.Handle);
                if (item == null)
                {
                    return;
                }

                item.IsAvailable = false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnEntityRemoved(EntityRemovedEventArgs e)
        {
            try
            {
                var physicalItem = e.Entity as PhysicalItem;
                if (physicalItem == null || physicalItem.Item == null)
                {
                    return;
                }

                var item = EntityManager9.GetAbilityFast(physicalItem.Item.Handle);
                if (item == null)
                {
                    return;
                }

                item.IsAvailable = true;
                //UpdateManager.BeginInvoke(100, () => InventoryChanged?.Invoke(null, EventArgs.Empty));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private IEnumerable<Item> GetInventoryItems(Inventory inventory)
        {
            foreach (var slot in this.inventoryItemSlots)
            {
                var item = inventory.GetItem(slot);

                if (item != null)
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<Item> GetStashItems(Inventory inventory)
        {
            foreach (var slot in this.stashItemSlots)
            {
                var item = inventory.GetItem(slot);

                if (item != null)
                {
                    yield return item;
                }
            }
        }

        private void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "m_bToggleState":
                    {
                        var newValue = e.NewValue.GetBoolean();
                        if (!newValue || newValue == e.OldValue.GetBoolean())
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            var ability = EntityManager9.GetAbilityFast(sender.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            this.AbilityCasted?.Invoke(ability);
                        });
                    }
                    break;
                case "m_bInAbilityPhase":
                    {
                        var newValue = e.NewValue.GetBoolean();
                        if (newValue == e.OldValue.GetBoolean())
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            var ability = EntityManager9.GetAbilityFast(sender.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            ability.IsCasting = newValue;
                            ability.Owner.IsCasting = newValue;

                            this.AbilityCastChange?.Invoke(ability);
                        });
                    }
                    break;
                case "m_flEnableTime":
                    {
                        var newValue = e.NewValue.GetSingle();
                        if (newValue == e.OldValue.GetSingle())
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            var ability = EntityManager9.GetAbilityFast(sender.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            ability.ItemEnableTimeSleeper.SleepUntil(newValue);
                        });
                    }
                    break;
                case "m_flCastStartTime":
                    {
                        var newValue = e.NewValue.GetSingle();
                        var oldValue = e.OldValue.GetSingle();
                        if (newValue == oldValue)
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            if (this.AbilityCasted == null)
                            {
                                return;
                            }

                            var ability = EntityManager9.GetAbilityFast(sender.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            if (!ability.IsDisplayingCharges)
                            {
                                return;
                            }

                            var castTime = newValue - oldValue;
                            if (castTime < 0 || oldValue < 0)
                            {
                                return;
                            }

                            var visibleTime = GameManager.RawGameTime - ability.Owner.LastNotVisibleTime;
                            if (visibleTime < 0.05f)
                            {
                                return;
                            }

                            if (ability.CastPoint <= 0)
                            {
                                this.AbilityCasted(ability);
                            }
                            else
                            {
                                if (Math.Abs(ability.CastPoint - castTime) < 0.03f)
                                {
                                    this.AbilityCasted(ability);
                                }
                            }
                        });
                    }
                    break;
                case "m_fCooldown":
                    {
                        var newValue = e.NewValue.GetSingle();
                        var oldValue = e.OldValue.GetSingle();
                        if (newValue == oldValue)
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            if (this.AbilityCasted == null)
                            {
                                return;
                            }

                            if (newValue <= oldValue || oldValue > 0)
                            {
                                return;
                            }

                            var ability = EntityManager9.GetAbilityFast(sender.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            var visibleTime = GameManager.RawGameTime - ability.Owner.LastNotVisibleTime;
                            if (visibleTime < 0.05f)
                            {
                                return;
                            }

                            this.AbilityCasted(ability);
                        });
                    }
                    break;
                case "m_flChannelStartTime":
                    {
                        var newValue = e.NewValue.GetSingle();
                        if (newValue == e.OldValue.GetSingle())
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            var ability = EntityManager9.GetAbilityFast(sender.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            if (!(ability is IChanneled channeled))
                            {
                                return;
                            }

                            if (newValue > 0)
                            {
                                ability.IsChanneling = true;
                                channeled.Owner.ChannelEndTime = newValue + channeled.ChannelTime;
                                channeled.Owner.ChannelActivatesOnCast = channeled.IsActivatesOnChannelStart;

                                this.AbilityChannel?.Invoke(ability);
                            }
                            else
                            {
                                ability.IsChanneling = false;
                                channeled.Owner.ChannelEndTime = 0;
                                channeled.Owner.ChannelActivatesOnCast = false;
                            }
                        });
                    }
                    break;
                case "m_iParity":
                    {
                        if (e.NewValue.GetInt32() == e.OldValue.GetInt32())
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            var owner = EntityManager9.GetUnitFast(sender.Handle);
                            
                            if (owner == null)
                            {
                                return;
                            }

                            var inventory = owner.BaseInventory;
                            var checkedItems = new List<uint>();

                            foreach (var handle in this.GetInventoryItems(inventory).Select(x => x.Handle))
                            {
                                var item = EntityManager9.GetAbilityFast(handle);
                                if (item == null)
                                {
                                    continue;
                                }

                                checkedItems.Add(handle);

                                if (item.Owner == owner)
                                {
                                    item.IsAvailable = true;
                                    //UpdateManager.BeginInvoke(100, () => InventoryChanged?.Invoke(null, EventArgs.Empty));
                                    continue;
                                }

                                EntityManager9.RemoveAbility(item.BaseAbility);
                                EntityManager9.AddAbility(item.BaseAbility);
                            }

                            foreach (var handle in this.GetStashItems(inventory).Select(x => x.Handle))
                            {
                                var item = EntityManager9.GetAbilityFast(handle);
                                if (item == null)
                                {
                                    continue;
                                }

                                checkedItems.Add(handle);

                                if (item.Owner == owner)
                                {
                                    item.IsAvailable = false;
                                    //UpdateManager.BeginInvoke(100, () => InventoryChanged?.Invoke(null, EventArgs.Empty));
                                    continue;
                                }

                                EntityManager9.RemoveAbility(item.BaseAbility);
                                EntityManager9.AddAbility(item.BaseAbility);
                            }

                            // stashed neutral items
                            foreach (var item in owner.AbilitiesFast.Where(x => x.IsItem && x.IsAvailable && !checkedItems.Contains(x.Handle)))
                            {
                                item.IsAvailable = false;
                                //UpdateManager.BeginInvoke(100, () => InventoryChanged?.Invoke(null, EventArgs.Empty));
                            }
                        });
                    }
                    break;
            }
        }

        private void UpdateItemState(Ability9 ability)
        {
            UpdateManager.BeginInvoke(500, () =>
            {
                try
                {
                    var owner = ability.Owner;
                    if (owner?.IsValid != true)
                    {
                        return;
                    }

                    ability.IsAvailable = this.GetInventoryItems(owner.BaseInventory).Any(x => x.Handle == ability.Handle);
                    //UpdateManager.BeginInvoke(100, () => InventoryChanged?.Invoke(null, EventArgs.Empty));
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            });
        }
    }
}
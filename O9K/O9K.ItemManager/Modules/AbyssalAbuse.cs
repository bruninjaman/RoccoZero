//namespace O9K.ItemManager.Modules
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    using Core.Entities.Heroes;
//    using Core.Entities.Units;
//    using Core.Helpers;
//    using Core.Logger;
//    using Core.Managers.Entity;
//    using Core.Managers.Menu;
//    using Core.Managers.Menu.EventArgs;
//    using Core.Managers.Menu.Items;

//    using Divine;

//    using Metadata;

//    internal class AbyssalAbuse : IModule
//    {
//        private const AbilityId abyssalId = AbilityId.item_silver_edge;

//        private readonly Sleeper attackSleeper = new Sleeper();

//        private readonly HashSet<AbilityId> disassembledAbyssalIds = new HashSet<AbilityId>
//        {
//            AbilityId.item_invis_sword,
//            AbilityId.item_echo_sabre,
//            AbilityId.item_recipe_silver_edge
//        };

//        private readonly MenuSwitcher enabled;

//        private readonly Sleeper sleeper = new Sleeper();

//        private Owner owner;

//        public AbyssalAbuse(IMainMenu mainMenu)
//        {
//            this.enabled = mainMenu.AbyssalAbuseMenu.Add(
//                new MenuSwitcher("Auto disassemble", false).SetTooltip("Auto disassemble to remove echo sabre passive"));
//            this.enabled.AddTranslation(Lang.Ru, "Авто разбор");
//            this.enabled.AddTooltipTranslation(Lang.Ru, "Автоматически разбирать, чтобы убрать кд пассивного баша");
//            this.enabled.AddTranslation(Lang.Cn, "自动分解");
//            this.enabled.AddTooltipTranslation(Lang.Cn, "自动分解以消除技能CD");
//        }

//        public void Activate()
//        {
//            this.owner = EntityManager9.Owner;
//            this.enabled.ValueChange += this.EnabledOnValueChange;
//        }

//        public void Dispose()
//        {
//            this.enabled.ValueChange -= this.EnabledOnValueChange;
//            UpdateManager.DestroyIngameUpdate(this.OnUpdate);
//            EntityManager9.UnitMonitor.AttackStart -= this.OnAttackStart;
//        }

//        private void Disassemble()
//        {
//            try
//            {
//                var baseHero = this.owner.Hero.BaseHero;
//                var abyssal = baseHero.Inventory.MainItems.FirstOrDefault(x => x.Id == abyssalId);
//                if (abyssal == null)
//                {
//                    return;
//                }

//                var freeSlots = baseHero.Inventory.FreeMainSlots.Count() + baseHero.Inventory.FreeBackpackSlots.Count();
//                if (freeSlots < this.disassembledAbyssalIds.Count - 1)
//                {
//                    return;
//                }

//                abyssal.Disassemble();
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//            }
//        }

//        private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
//        {
//            if (e.NewValue)
//            {
//                UpdateManager.CreateIngameUpdate(100, this.OnUpdate);
//                EntityManager9.UnitMonitor.AttackStart += this.OnAttackStart;
//            }
//            else
//            {
//                UpdateManager.DestroyIngameUpdate(this.OnUpdate);
//                EntityManager9.UnitMonitor.AttackStart -= this.OnAttackStart;
//            }
//        }

//        private void OnAttackStart(Unit9 unit)
//        {
//            try
//            {
//                if (!unit.IsMyHero || this.attackSleeper)
//                {
//                    return;
//                }

//                if (unit.Target?.IsCreep != false)
//                {
//                    return;
//                }

//                var attackPoint = unit.GetAttackPoint();
//                var attackCheckDelay = attackPoint * 3f;
//                var disassembleDelay = attackPoint * 1.5f;
//                var assembleDelay = attackPoint * 2f;

//                this.attackSleeper.Sleep(attackCheckDelay);
//                this.sleeper.Sleep(assembleDelay);

//                UpdateManager.BeginInvoke((int)(disassembleDelay * 1000), this.Disassemble);
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//            }
//        }

//        private void OnUpdate()
//        {
//            try
//            {
//                if (this.sleeper)
//                {
//                    return;
//                }

//                var hero = this.owner.Hero;
//                if (hero == null || !hero.IsValid || !hero.IsAlive)
//                {
//                    return;
//                }

//                var baseHero = hero.BaseHero;
//                var items = baseHero.Inventory.MainItems.Concat(baseHero.Inventory.BackpackItems).ToList();
//                var disassembledAbyssal = items.Where(x => this.disassembledAbyssalIds.Contains(x.Id)).ToList();
//                if (disassembledAbyssal.Count != this.disassembledAbyssalIds.Count)
//                {
//                    return;
//                }

//                var delay = -75;

//                foreach (var disassembled in disassembledAbyssal)
//                {
//                    if (!disassembled.IsCombineLocked)
//                    {
//                        continue;
//                    }

//                    UpdateManager.BeginInvoke(delay += 75, () => disassembled.CombineUnlock());
//                }

//                this.sleeper.Sleep(0.5f);
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//            }
//        }
//    }
//}
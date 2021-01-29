namespace O9K.Core.Entities.Heroes.Unique
{
    using System;

    using Divine;

    using Helpers;

    using Logger;

    using Managers.Entity;

    using Metadata;

    using Units;

    [HeroId(HeroId.npc_dota_hero_windrunner)]
    public class Windranger : Hero9, IDisposable
    {
        private readonly float focusFireAttackSpeed;

        public Windranger(Hero baseHero)
            : base(baseHero)
        {
            this.focusFireAttackSpeed = new SpecialData(AbilityId.windrunner_focusfire, "bonus_attack_speed").GetValue(1);

            if (this.IsControllable)
            {
                OrderManager.OrderAdding += this.OnOrderAdding;
            }
        }

        public bool FocusFireActive { get; private set; }

        public Unit9 FocusFireTarget { get; private set; }

        public void Dispose()
        {
            OrderManager.OrderAdding -= this.OnOrderAdding;
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
        }

        internal override float GetAttackSpeed(Unit9 target = null)
        {
            var attackSpeed = base.GetAttackSpeed(target);
            if (target == null || !this.FocusFireActive || this.FocusFireTarget?.Handle == target.Handle)
            {
                return attackSpeed;
            }

            return attackSpeed - this.focusFireAttackSpeed;
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                var order = e.Order;
                if (order.Type != OrderType.CastTarget || order.Ability.Id != AbilityId.windrunner_focusfire || !e.Process)
                {
                    return;
                }

                this.FocusFireTarget = EntityManager9.GetUnit(order.Target.Handle);
                if (this.FocusFireTarget == null || this.FocusFireTarget.IsLinkensProtected || this.FocusFireTarget.IsSpellShieldProtected)
                {
                    return;
                }

                ModifierManager.ModifierAdded += this.OnModifierAdded;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            try
            {
                var modifier = e.Modifier;
                if (modifier.Owner.Handle != this.Handle || modifier.Name != "modifier_windrunner_focusfire")
                {
                    return;
                }

                this.FocusFireActive = true;

                ModifierManager.ModifierAdded -= this.OnModifierAdded;
                ModifierManager.ModifierRemoved += this.OnModifierRemoved;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnModifierRemoved(ModifierRemovedEventArgs e)
        {
            try
            {
                var modifier = e.Modifier;
                if (modifier.Owner.Handle != this.Handle || modifier.Name != "modifier_windrunner_focusfire")
                {
                    return;
                }

                this.FocusFireActive = false;

                ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
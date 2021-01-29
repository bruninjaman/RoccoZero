namespace O9K.Core.Entities.Abilities.Items
{
    using System;

    using Base;
    using Base.Components;
    using Base.Types;

    using Divine;

    using Entities.Units;

    using Helpers;
    using Helpers.Range;

    using Logger;

    using Managers.Entity;

    using Metadata;

    using SharpDX;

    [AbilityId(AbilityId.item_hurricane_pike)]
    public class HurricanePike : RangedAbility, IBlink, IHasRangeIncrease
    {
        private readonly SpecialData attackRange;

        private Unit9 pikeTarget;

        private bool subbed;

        public HurricanePike(Ability baseAbility)
            : base(baseAbility)
        {
            this.attackRange = new SpecialData(baseAbility, "base_attack_range");
            this.RangeData = new SpecialData(baseAbility, "push_length");
        }

        public BlinkType BlinkType { get; } = BlinkType.Leap;

        public bool IsRangeIncreasePermanent { get; } = true;

        public override float Range
        {
            get
            {
                return this.RangeData.GetValue(this.Level);
            }
        }

        public RangeIncreaseType RangeIncreaseType { get; } = RangeIncreaseType.Attack;

        public string RangeModifierName { get; } = "modifier_item_hurricane_pike";

        public override float Speed { get; } = 1200;

        public override bool CanHit(Unit9 target)
        {
            if (!base.CanHit(target))
            {
                return false;
            }

            if (!this.Owner.IsAlly(target) && this.Owner.Distance(target) > this.CastRange / 2)
            {
                return false;
            }

            return true;
        }

        public override void Dispose()
        {
            base.Dispose();

            OrderManager.OrderAdding -= this.OnOrderAdding;
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
        }

        public override float GetHitTime(Vector3 position)
        {
            return this.GetCastDelay(position) + this.ActivationDelay + (this.Range / this.Speed);
        }

        public float GetRangeIncrease(Unit9 unit, RangeIncreaseType type)
        {
            if (!this.IsUsable || !this.Owner.IsRanged)
            {
                return 0;
            }

            return this.attackRange.GetValue(this.Level);
        }

        internal override void SetOwner(Unit9 owner)
        {
            base.SetOwner(owner);

            if (this.IsControllable)
            {
                OrderManager.OrderAdding += this.OnOrderAdding;
            }
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                var order = e.Order;
                if (this.subbed || order.Type != OrderType.CastTarget || order.Ability.Id != this.Id || !e.Process)
                {
                    return;
                }

                this.pikeTarget = EntityManager9.GetUnit(order.Target.Handle);
                if (this.pikeTarget == null || this.pikeTarget.IsLinkensProtected || this.pikeTarget.IsSpellShieldProtected)
                {
                    return;
                }

                ModifierManager.ModifierAdded += this.OnModifierAdded;
                this.subbed = true;
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
                if (modifier.Owner.Handle != this.Owner.Handle || modifier.Name != "modifier_item_hurricane_pike_range")
                {
                    return;
                }

                this.Owner.HurricanePikeTarget = this.pikeTarget;

                ModifierManager.ModifierAdded -= this.OnModifierAdded;
                ModifierManager.ModifierRemoved += this.OnModifierRemoved;
            }
            catch (Exception ex)
            {
                ModifierManager.ModifierAdded -= this.OnModifierAdded;
                Logger.Error(ex);
            }
        }

        private void OnModifierRemoved(ModifierRemovedEventArgs e)
        {
            try
            {
                var modifier = e.Modifier;
                if (modifier.Owner.Handle != this.Owner.Handle || modifier.Name != "modifier_item_hurricane_pike_range")
                {
                    return;
                }

                this.Owner.HurricanePikeTarget = null;

                ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
                this.subbed = false;
            }
            catch (Exception ex)
            {
                ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
                Logger.Error(ex);
            }
        }
    }
}
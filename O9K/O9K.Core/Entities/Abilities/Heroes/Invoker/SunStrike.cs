namespace O9K.Core.Entities.Abilities.Heroes.Invoker
{
    using System.Collections.Generic;

    using Base;
    using Base.Types;

    using Core.Helpers;
    using Core.Helpers.Damage;

    using Divine;

    using Entities.Units;

    using Helpers;

    using Metadata;

    using SharpDX;

    [AbilityId(AbilityId.invoker_sun_strike)]
    public class SunStrike : CircleAbility, IInvokableAbility, INuke
    {
        private readonly InvokeHelper<SunStrike> invokeHelper;

        public SunStrike(Ability baseAbility)
            : base(baseAbility)
        {
            //todo unit collision ?
            this.invokeHelper = new InvokeHelper<SunStrike>(this);

            this.ActivationDelayData = new SpecialData(baseAbility, "delay");
            this.RadiusData = new SpecialData(baseAbility, "area_of_effect");
            this.DamageData = new SpecialData(baseAbility, "damage");
        }

        public bool CanBeInvoked
        {
            get
            {
                if (this.IsInvoked)
                {
                    return true;
                }

                return this.invokeHelper.CanInvoke(false);
            }
        }

        public AbilitySlot GetAbilitySlot => invokeHelper.GetAbilitySlot;
        
        public override float CastRange { get; } = 9999999;

        public bool IsInvoked => this.invokeHelper.IsInvoked;
        public bool IsInvokedOnLastSlot => IsInvoked && GetAbilitySlot  == AbilitySlot.Slot_5;

        public override bool IsUsable
        {
            get
            {
                if (!this.IsAvailable)
                {
                    return false;
                }

                return true;
            }
        }

        public AbilityId[] RequiredOrbs { get; } = { AbilityId.invoker_exort, AbilityId.invoker_exort, AbilityId.invoker_exort };

        public override bool CanBeCasted(bool checkChanneling = true)
        {
            return base.CanBeCasted(checkChanneling) && this.invokeHelper.CanInvoke(!this.IsInvoked);
        }

        public override Damage GetRawDamage(Unit9 unit, float? remainingHealth = null)
        {
            return new Damage
            {
                [this.DamageType] = this.DamageData.GetValue(this.invokeHelper.Exort.Level)
            };
        }

        public bool Invoke(List<AbilityId> currentOrbs = null, bool queue = false, bool bypass = false, bool invokeIfOnLastPosition = false)
        {
            return this.invokeHelper.Invoke(currentOrbs, queue, bypass, invokeIfOnLastPosition);
        }

        public override bool UseAbility(Vector3 position, bool queue = false, bool bypass = false)
        {
            return this.Invoke(null, false, bypass) && base.UseAbility(position, queue, bypass);
        }

        internal override void SetOwner(Unit9 owner)
        {
            base.SetOwner(owner);
            this.invokeHelper.SetOwner(owner);
        }
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.Hoodwink
{
    using Base;
    using Base.Types;

    using Divine;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.hoodwink_sharpshooter)]
    public class Sharpshooter : LineAbility, INuke, IDebuff
    {
        private readonly SpecialData maxChargeTime;

        public Sharpshooter(Ability baseAbility)
            : base(baseAbility)
        {
            this.maxChargeTime = new SpecialData(baseAbility, "max_charge_time");
            this.RadiusData = new SpecialData(baseAbility, "arrow_width");
            this.SpeedData = new SpecialData(baseAbility, "arrow_speed");
            this.DamageData = new SpecialData(baseAbility, "max_damage");
        }

        public override float ActivationDelay
        {
            get
            {
                return this.maxChargeTime.GetValue(0);
            }
        }

        public override bool HasAreaOfEffect { get; } = false;

        public string DebuffModifierName { get; } = "modifier_hoodwink_sharpshooter_debuff";

        /*public int GetCurrentDamage(Unit9 unit)
        {
            return (int)(this.GetDamage(unit) * (this.BaseAbility.ChannelTime / this.ActivationDelay));
        }

        public override Damage GetRawDamage(Unit9 unit, float? remainingHealth = null)
        {
            //todo improve damage reduction

            var damage = base.GetRawDamage(unit, remainingHealth);

            damage[this.DamageType] *= 0.8f;

            return damage;
        }*/
    }
}
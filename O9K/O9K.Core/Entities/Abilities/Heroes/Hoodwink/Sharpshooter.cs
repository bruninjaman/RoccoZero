namespace O9K.Core.Entities.Abilities.Heroes.Hoodwink;

using Base;
using Base.Components;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.hoodwink_sharpshooter)]
public class Sharpshooter : LineAbility, INuke, IDebuff, IChanneled
{
    private readonly SpecialData maxChargeTime;

    public Sharpshooter(Ability baseAbility)
        : base(baseAbility)
    {
        this.maxChargeTime = new SpecialData(baseAbility, "max_charge_time");
        this.RadiusData = new SpecialData(baseAbility, "arrow_width");
        this.SpeedData = new SpecialData(baseAbility, "arrow_speed");
        this.DamageData = new SpecialData(baseAbility, "max_damage");
        this.RangeData = new SpecialData(baseAbility, "arrow_range");

        this.ChannelTime = this.maxChargeTime.GetValue(1) + 2f;

    }

    public override float Range
    {
        get
        {
            return  this.RangeData.GetValue(this.Level);
        }
    }

    public override float Speed
    {
        get
        {
            return this.SpeedData.GetValue(this.Level);
        }
    }

    public override bool HasAreaOfEffect { get; } = false;

    public float ChannelTime { get; }

    public bool IsActivatesOnChannelStart { get; } = true;

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
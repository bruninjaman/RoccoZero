using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Metadata;
using O9K.Core.Entities.Units;
using O9K.Core.Helpers;
using O9K.Core.Helpers.Damage;

namespace O9K.Core.Entities.Abilities.Heroes.PhantomAssassin;

[AbilityId(AbilityId.phantom_assassin_fan_of_knives)]
public class FanOfKnives : AreaOfEffectAbility, INuke
{
    public FanOfKnives(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.DamageData = new SpecialData(baseAbility, "pct_health_damage_initial");
        this.SpeedData = new SpecialData(baseAbility, "projectile_speed");
    }

    public override DamageType DamageType { get; } = DamageType.Pure;

    public override Damage GetRawDamage(Unit9 unit, float? remainingHealth = null)
    {
        var health = unit.MaximumHealth;

        return new Damage
        {
            [this.DamageType] = (int)((health * this.DamageData.GetValue(this.Level)) / 100)
        };
    }
}
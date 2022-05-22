namespace O9K.Core.Entities.Abilities.Heroes.Necrophos;

using System.Linq;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Entities.Units;

using Helpers.Damage;

using Metadata;

using O9K.Core.Helpers;
using O9K.Core.Managers.Entity;

[AbilityId(AbilityId.necrolyte_death_seeker)]
public class DeathSeeker : RangedAbility, INuke
{
    private readonly SpecialData speedMultiplierData;

    private DeathPulse deathPulse;

    public DeathSeeker(Ability baseAbility)
        : base(baseAbility)
    {
        this.speedMultiplierData = new SpecialData(baseAbility, "projectile_multiplier");
    }

    public override float Speed
    {
        get
        {
            var speed = this.deathPulse.Speed;
            var multiplier = this.speedMultiplierData.GetValue(this.Level);

            return speed + (speed * (multiplier / 100f));
        }
    }

    public override Damage GetRawDamage(Unit9 unit, float? remainingHealth = null)
    {
        return deathPulse.GetRawDamage(unit, remainingHealth);
    }

    internal override void SetOwner(Unit9 owner)
    {
        base.SetOwner(owner);

        var ability = EntityManager9.BaseAbilities.FirstOrDefault(
            x => x.Id == AbilityId.necrolyte_death_pulse && x.Owner?.Handle == owner.Handle);

        if (ability == null)
        {
            return;
        }

        this.deathPulse = (DeathPulse)EntityManager9.AddAbility(ability);
    }
}
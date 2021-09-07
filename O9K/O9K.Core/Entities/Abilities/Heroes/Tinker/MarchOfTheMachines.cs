namespace O9K.Core.Entities.Abilities.Heroes.Tinker;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Entities.Units;

using Helpers;

using Metadata;

[AbilityId(AbilityId.tinker_march_of_the_machines)]
public class MarchOfTheMachines : LineAbility
{
    public MarchOfTheMachines(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.SpeedData = new SpecialData(baseAbility, "speed");
        this.DamageData = new SpecialData(baseAbility, "damage");
    }

    public override int GetDamage(Unit9 unit)
    {
        return (int)this.DamageData.GetValue(this.Level);
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.Riki;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.riki_poison_dart)]
public class SleepingDart : RangedAbility, IDisable, INuke
{

    public SleepingDart(Ability baseAbility)
        : base(baseAbility)
    {
        this.SpeedData = new SpecialData(baseAbility, "projectile_speed");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}
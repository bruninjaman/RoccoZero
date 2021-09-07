namespace O9K.Core.Entities.Abilities.Heroes.Warlock;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.warlock_rain_of_chaos)]
public class ChaoticOffering : CircleAbility, IDisable
{
    public ChaoticOffering(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "aoe");
    }

    public override float ActivationDelay { get; } = 0.5f;

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}
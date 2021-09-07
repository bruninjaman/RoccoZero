namespace O9K.Core.Entities.Abilities.Heroes.Jakiro;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.jakiro_ice_path)]
public class IcePath : LineAbility, IDisable
{
    public IcePath(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "path_radius");
        this.ActivationDelayData = new SpecialData(baseAbility, "path_delay");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}
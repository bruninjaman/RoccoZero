namespace O9K.Core.Entities.Abilities.Heroes.Luna;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.luna_lucent_beam)]
public class LucentBeam : RangedAbility, INuke, IDisable
{
    public LucentBeam(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "beam_damage");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}
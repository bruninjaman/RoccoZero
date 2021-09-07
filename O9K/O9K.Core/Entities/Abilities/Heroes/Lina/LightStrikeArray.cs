namespace O9K.Core.Entities.Abilities.Heroes.Lina;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.lina_light_strike_array)]
public class LightStrikeArray : CircleAbility, IDisable, INuke
{
    public LightStrikeArray(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "light_strike_array_aoe");
        this.ActivationDelayData = new SpecialData(baseAbility, "light_strike_array_delay_time");
        this.DamageData = new SpecialData(baseAbility, "light_strike_array_damage");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}
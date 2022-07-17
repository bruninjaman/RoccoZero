using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Metadata;
using O9K.Core.Helpers;

namespace O9K.Core.Entities.Abilities.Heroes.Sniper;

[AbilityId(AbilityId.sniper_concussive_grenade)]
public class ConcussiveGrenade : CircleAbility, IDisable, INuke
{
    public ConcussiveGrenade(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.DamageData = new SpecialData(baseAbility, "damage");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Disarmed;
}

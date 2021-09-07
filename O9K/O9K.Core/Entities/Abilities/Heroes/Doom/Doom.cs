namespace O9K.Core.Entities.Abilities.Heroes.Doom;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.doom_bringer_doom)]
public class Doom : RangedAbility, IDisable
{
    public Doom(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = UnitState.Silenced | UnitState.Muted;
}
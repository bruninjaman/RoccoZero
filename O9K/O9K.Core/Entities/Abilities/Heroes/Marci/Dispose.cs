namespace O9K.Core.Entities.Abilities.Heroes.Marci;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.marci_grapple)]
public class Dispose : RangedAbility, IDisable, INuke
{
    public Dispose(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;

    public override bool TargetsAlly { get; } = true;

    public override bool TargetsEnemy { get; } = true;
}
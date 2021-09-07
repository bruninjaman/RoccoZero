namespace O9K.Core.Entities.Abilities.Base.Types;

using Components.Base;

using Divine.Entity.Entities.Units.Components;

public interface IDisable : IActiveAbility
{
    UnitState AppliesUnitState { get; }
}
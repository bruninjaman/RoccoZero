namespace O9K.Core.Entities.Abilities.Base.Components;

using Divine.Entity.Entities.Abilities.Components;

public interface IUpgradable
{
    AbilityId UpgradedBy { get; }
}
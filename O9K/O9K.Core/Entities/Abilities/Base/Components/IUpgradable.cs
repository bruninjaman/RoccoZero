namespace O9K.Core.Entities.Abilities.Base.Components
{
    using Divine;

    public interface IUpgradable
    {
        AbilityId UpgradedBy { get; }
    }
}
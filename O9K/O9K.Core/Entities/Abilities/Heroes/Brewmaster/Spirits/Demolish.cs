namespace O9K.Core.Entities.Abilities.Heroes.Brewmaster.Spirits
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.brewmaster_earth_pulverize)]
    public class Demolish : PassiveAbility
    {
        public Demolish(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
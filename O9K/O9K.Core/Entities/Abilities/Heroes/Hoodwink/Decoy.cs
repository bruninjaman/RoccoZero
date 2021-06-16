namespace O9K.Core.Entities.Abilities.Heroes.Hoodwink
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.hoodwink_decoy)]
    public class Decoy : ActiveAbility
    {
        public Decoy(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
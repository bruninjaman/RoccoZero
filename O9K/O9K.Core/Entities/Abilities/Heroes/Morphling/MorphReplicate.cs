namespace O9K.Core.Entities.Abilities.Heroes.Morphling
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.morphling_morph_replicate)]
    public class MorphReplicate : ActiveAbility
    {
        public MorphReplicate(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
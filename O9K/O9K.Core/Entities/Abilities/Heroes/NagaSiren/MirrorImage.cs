namespace O9K.Core.Entities.Abilities.Heroes.NagaSiren
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.naga_siren_mirror_image)]
    public class MirrorImage : ActiveAbility
    {
        public MirrorImage(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.ShadowShaman
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.shadow_shaman_mass_serpent_ward)]
    public class MassSerpentWard : CircleAbility
    {
        public MassSerpentWard(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public override float Radius { get; } = 200;
    }
}
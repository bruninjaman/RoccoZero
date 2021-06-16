namespace O9K.Core.Entities.Abilities.Heroes.Venomancer
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.venomancer_plague_ward)]
    public class PlagueWard : CircleAbility
    {
        public PlagueWard(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public override float Radius { get; } = 600;
    }
}
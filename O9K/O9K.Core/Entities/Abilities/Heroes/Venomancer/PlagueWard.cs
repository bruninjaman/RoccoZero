namespace O9K.Core.Entities.Abilities.Heroes.Venomancer
{
    using Base;

    using Divine;

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
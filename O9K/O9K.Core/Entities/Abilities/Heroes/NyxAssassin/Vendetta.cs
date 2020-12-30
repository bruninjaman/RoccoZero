namespace O9K.Core.Entities.Abilities.Heroes.NyxAssassin
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.nyx_assassin_vendetta)]
    public class Vendetta : ActiveAbility
    {
        public Vendetta(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public override bool IsInvisibility { get; } = true;
    }
}
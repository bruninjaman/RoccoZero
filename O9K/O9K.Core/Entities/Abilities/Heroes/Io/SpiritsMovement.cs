namespace O9K.Core.Entities.Abilities.Heroes.Io
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.wisp_spirits_in)]
    public class SpiritsMovement : ActiveAbility
    {
        public SpiritsMovement(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
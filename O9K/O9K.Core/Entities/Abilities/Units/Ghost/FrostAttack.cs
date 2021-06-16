namespace O9K.Core.Entities.Abilities.Units.Ghost
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.ghost_frost_attack)]
    public class FrostAttack : PassiveAbility
    {
        public FrostAttack(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
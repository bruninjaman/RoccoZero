namespace O9K.Core.Entities.Abilities.Heroes.Clinkz
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.clinkz_death_pact)]
    public class DeathPact : RangedAbility
    {
        public DeathPact(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
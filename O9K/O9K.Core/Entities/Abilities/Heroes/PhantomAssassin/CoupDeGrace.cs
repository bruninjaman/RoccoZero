namespace O9K.Core.Entities.Abilities.Heroes.PhantomAssassin
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.phantom_assassin_coup_de_grace)]
    public class CoupDeGrace : PassiveAbility
    {
        public CoupDeGrace(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
namespace O9K.Core.Entities.Abilities.Units.CentaurCourser
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.mudgolem_cloak_aura)]
    public class CloakAura : PassiveAbility
    {
        public CloakAura(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
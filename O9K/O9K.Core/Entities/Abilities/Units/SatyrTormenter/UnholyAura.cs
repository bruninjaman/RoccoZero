namespace O9K.Core.Entities.Abilities.Units.SatyrTormenter
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.satyr_hellcaller_unholy_aura)]
    public class UnholyAura : PassiveAbility
    {
        public UnholyAura(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
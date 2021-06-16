namespace O9K.Core.Entities.Abilities.Units.AncientGraniteGolem
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.granite_golem_hp_aura)]
    public class GraniteAura : PassiveAbility
    {
        public GraniteAura(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
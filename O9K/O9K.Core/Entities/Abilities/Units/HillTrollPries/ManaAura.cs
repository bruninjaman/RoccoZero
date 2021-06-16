namespace O9K.Core.Entities.Abilities.Units.HillTrollPries
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.forest_troll_high_priest_mana_aura)]
    public class ManaAura : PassiveAbility
    {
        public ManaAura(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
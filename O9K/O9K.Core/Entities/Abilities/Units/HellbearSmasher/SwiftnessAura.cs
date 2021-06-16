namespace O9K.Core.Entities.Abilities.Units.HellbearSmasher
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.centaur_khan_endurance_aura)]
    public class SwiftnessAura : PassiveAbility
    {
        public SwiftnessAura(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
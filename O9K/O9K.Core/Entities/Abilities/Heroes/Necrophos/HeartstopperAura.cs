namespace O9K.Core.Entities.Abilities.Heroes.Necrophos
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.necrolyte_heartstopper_aura)]
    public class HeartstopperAura : PassiveAbility
    {
        public HeartstopperAura(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
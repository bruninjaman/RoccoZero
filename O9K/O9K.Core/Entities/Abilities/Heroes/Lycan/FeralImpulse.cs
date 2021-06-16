namespace O9K.Core.Entities.Abilities.Heroes.Lycan
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.lycan_feral_impulse)]
    public class FeralImpulse : PassiveAbility
    {
        public FeralImpulse(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.Invoker.ForgedSpirit
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.forged_spirit_melting_strike)]
    public class MeltingStrike : PassiveAbility
    {
        public MeltingStrike(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
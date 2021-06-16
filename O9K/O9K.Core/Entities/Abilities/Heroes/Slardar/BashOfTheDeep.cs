namespace O9K.Core.Entities.Abilities.Heroes.Slardar
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.slardar_bash)]
    public class BashOfTheDeep : PassiveAbility
    {
        public BashOfTheDeep(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
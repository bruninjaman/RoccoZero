namespace O9K.Core.Entities.Abilities.Heroes.Meepo
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.meepo_divided_we_stand)]
    public class DividedWeStand : PassiveAbility
    {
        public DividedWeStand(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
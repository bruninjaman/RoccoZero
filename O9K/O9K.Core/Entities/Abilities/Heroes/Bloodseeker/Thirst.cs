namespace O9K.Core.Entities.Abilities.Heroes.Bloodseeker
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.bloodseeker_thirst)]
    public class Thirst : PassiveAbility
    {
        public Thirst(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
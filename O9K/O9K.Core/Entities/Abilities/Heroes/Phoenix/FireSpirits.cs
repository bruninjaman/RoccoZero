namespace O9K.Core.Entities.Abilities.Heroes.Phoenix
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.phoenix_fire_spirits)]
    public class FireSpirits : ActiveAbility
    {
        public FireSpirits(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
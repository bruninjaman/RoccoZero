namespace O9K.Core.Entities.Abilities.Heroes.Phoenix
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.phoenix_icarus_dive_stop)]
    public class IcarusDiveStop : ActiveAbility
    {
        public IcarusDiveStop(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
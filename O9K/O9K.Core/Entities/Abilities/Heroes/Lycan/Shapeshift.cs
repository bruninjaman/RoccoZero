namespace O9K.Core.Entities.Abilities.Heroes.Lycan
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.lycan_shapeshift)]
    public class Shapeshift : ActiveAbility
    {
        public Shapeshift(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.ShadowFiend
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.nevermore_necromastery)]
    public class Necromastery : PassiveAbility
    {
        public Necromastery(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.Viper
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.viper_corrosive_skin)]
    public class CorrosiveSkin : PassiveAbility
    {
        public CorrosiveSkin(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
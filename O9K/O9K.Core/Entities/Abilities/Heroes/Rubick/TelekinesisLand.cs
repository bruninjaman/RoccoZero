namespace O9K.Core.Entities.Abilities.Heroes.Rubick
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.rubick_telekinesis_land)]
    public class TelekinesisLand : CircleAbility
    {
        public TelekinesisLand(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
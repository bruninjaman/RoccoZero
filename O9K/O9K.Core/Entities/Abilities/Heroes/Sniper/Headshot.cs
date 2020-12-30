namespace O9K.Core.Entities.Abilities.Heroes.Sniper
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.sniper_headshot)]
    public class Headshot : PassiveAbility
    {
        public Headshot(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
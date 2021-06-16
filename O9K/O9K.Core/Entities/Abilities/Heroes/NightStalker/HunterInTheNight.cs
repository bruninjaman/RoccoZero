namespace O9K.Core.Entities.Abilities.Heroes.NightStalker
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.night_stalker_hunter_in_the_night)]
    public class HunterInTheNight : PassiveAbility
    {
        public HunterInTheNight(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
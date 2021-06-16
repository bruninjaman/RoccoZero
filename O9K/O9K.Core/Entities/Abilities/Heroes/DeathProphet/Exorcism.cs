namespace O9K.Core.Entities.Abilities.Heroes.DeathProphet
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.death_prophet_exorcism)]
    public class Exorcism : AreaOfEffectAbility
    {
        public Exorcism(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
            this.SpeedData = new SpecialData(baseAbility, "spirit_speed");
        }
    }
}
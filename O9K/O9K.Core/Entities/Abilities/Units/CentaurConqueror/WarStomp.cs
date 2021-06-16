namespace O9K.Core.Entities.Abilities.Units.CentaurConqueror
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.centaur_khan_war_stomp)]
    public class WarStomp : AreaOfEffectAbility, IDisable
    {
        public WarStomp(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Stunned;
    }
}
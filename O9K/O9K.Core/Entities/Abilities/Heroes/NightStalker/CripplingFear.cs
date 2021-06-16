namespace O9K.Core.Entities.Abilities.Heroes.NightStalker
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.night_stalker_crippling_fear)]
    public class CripplingFear : AreaOfEffectAbility, IDisable
    {
        public CripplingFear(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Silenced;
    }
}
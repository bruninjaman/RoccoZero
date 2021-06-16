namespace O9K.Core.Entities.Abilities.Heroes.Riki
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.riki_smoke_screen)]
    public class SmokeScreen : CircleAbility, IDisable
    {
        public SmokeScreen(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Silenced;
    }
}
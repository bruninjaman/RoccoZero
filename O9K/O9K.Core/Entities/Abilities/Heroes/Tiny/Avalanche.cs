namespace O9K.Core.Entities.Abilities.Heroes.Tiny
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.tiny_avalanche)]
    public class Avalanche : CircleAbility, IDisable, INuke
    {
        public Avalanche(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
            this.DamageData = new SpecialData(baseAbility, "avalanche_damage");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Stunned;
    }
}
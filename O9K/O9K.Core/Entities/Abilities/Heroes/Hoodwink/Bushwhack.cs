namespace O9K.Core.Entities.Abilities.Heroes.Hoodwink
{
    using System;

    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Types;

    [AbilityId(AbilityId.hoodwink_bushwhack)]
    public class Bushwhack : CircleAbility, IDisable
    {
        public Bushwhack(Ability baseAbility)
            : base(baseAbility)
        {
            this.SpeedData = new SpecialData(baseAbility, "projectile_speed");
            this.RadiusData = new SpecialData(baseAbility, "trap_radius");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Stunned;
    }
}
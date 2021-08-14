﻿namespace O9K.Core.Entities.Abilities.Items
{
    using Base;
    using Base.Components;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.item_wind_waker)]
    public class WindWaker : RangedAbility, IDisable, IShield, IAppliesImmobility
    {
        public WindWaker(Ability baseAbility)
            : base(baseAbility)
        {
            this.DurationData = new SpecialData(baseAbility, "cyclone_duration");
        }

        public string ImmobilityModifierName { get; } = "modifier_wind_waker";

        public UnitState AppliesUnitState { get; } = UnitState.Invulnerable;

        public string ShieldModifierName { get; } = "modifier_wind_waker";

        public bool ShieldsAlly { get; } = true;

        public bool ShieldsOwner { get; } = true;
    }
}
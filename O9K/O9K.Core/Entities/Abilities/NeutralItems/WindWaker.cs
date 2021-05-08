namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;
    using Base.Types;

    using Divine;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.item_wind_waker)]
    public class WindWaker : RangedAbility, IDisable, IShield
    {
        public WindWaker(Ability baseAbility)
            : base(baseAbility)
        {
            this.DurationData = new SpecialData(baseAbility, "cyclone_duration");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Invulnerable;

        public string ShieldModifierName { get; } = "modifier_wind_waker";

        public bool ShieldsAlly { get; } = true;

        public bool ShieldsOwner { get; } = true;
    }
}
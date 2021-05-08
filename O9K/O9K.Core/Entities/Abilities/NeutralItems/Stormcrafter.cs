namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;
    using Base.Components;
    using Base.Types;

    using Divine;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.item_stormcrafter)]
    public class Stormcrafter : ActiveAbility, IShield, IAppliesImmobility
    {
        public Stormcrafter(Ability baseAbility)
            : base(baseAbility)
        {
            this.DurationData = new SpecialData(baseAbility, "cyclone_duration");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Invulnerable;

        public string ImmobilityModifierName { get; } = "modifier_eul_cyclone";

        public string ShieldModifierName { get; } = "modifier_eul_cyclone";

        public bool ShieldsAlly { get; } = false;

        public bool ShieldsOwner { get; } = true;
    }
}
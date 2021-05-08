namespace O9K.Core.Entities.Abilities.Items
{
    using Base;
    using Base.Components;
    using Base.Types;

    using Divine;

    using Metadata;

    using O9K.Core.Helpers;

    [AbilityId(AbilityId.item_gungir)]
    public class Gleipnir : CircleAbility, IDisable, IAppliesImmobility
    {
        public Gleipnir(Ability baseAbility)
            : base(baseAbility)
        {
            this.DamageData = new SpecialData(baseAbility, "active_damage");
            this.RadiusData = new SpecialData(baseAbility, "radius");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Rooted;

        public string ImmobilityModifierName { get; } = "modifier_gungnir_debuff";

        public override float Speed { get; } = 1750;
    }
}
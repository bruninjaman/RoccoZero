namespace O9K.Core.Entities.Abilities.Heroes.Leshrac
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.leshrac_lightning_storm)]
    public class LightningStorm : RangedAbility, INuke, IDebuff
    {
        public LightningStorm(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
            this.DamageData = new SpecialData(baseAbility, "damage");
        }

        public string DebuffModifierName { get; } = "modifier_leshrac_lightning_storm_slow";
    }
}
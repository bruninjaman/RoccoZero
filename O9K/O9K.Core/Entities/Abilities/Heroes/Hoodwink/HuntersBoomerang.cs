namespace O9K.Core.Entities.Abilities.Heroes.Hoodwink
{
    using Base;

    using Divine;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Components;
    using O9K.Core.Entities.Abilities.Base.Types;
    using O9K.Core.Helpers;

    [AbilityId(AbilityId.hoodwink_hunters_boomerang)]
    public class HuntersBoomerang : RangedAbility, INuke, IDebuff, IAppliesImmobility
    {
        public HuntersBoomerang(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
            this.DamageData = new SpecialData(baseAbility, "damage");
        }

        public string DebuffModifierName { get; } = "modifier_hoodwink_acorn_shot_slow";

        public string ImmobilityModifierName { get; } = "modifier_hoodwink_acorn_shot_slow";
    }
}
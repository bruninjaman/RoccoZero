namespace O9K.Core.Entities.Abilities.Heroes.DarkWillow
{
    using Base;

    using Divine;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Types;
    using O9K.Core.Helpers;

    [AbilityId(AbilityId.dawnbreaker_fire_wreath)]
    public class Starbreaker : CircleAbility, INuke, IDebuff
    {
        public Starbreaker(Ability baseAbility)
            : base(baseAbility)
        {
            this.RangeData = new SpecialData(baseAbility, "smash_distance_from_hero");
            this.RadiusData = new SpecialData(baseAbility, "smash_radius");
            this.DamageData = new SpecialData(baseAbility, "smash_damage");
        }

        public string DebuffModifierName { get; } = "modifier_dawnbreaker_fire_wreath_smash_stun";
    }
}
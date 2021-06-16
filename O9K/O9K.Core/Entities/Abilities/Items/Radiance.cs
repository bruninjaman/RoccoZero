namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.item_radiance)]
    public class Radiance : ToggleAbility
    {
        public Radiance(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "aura_radius");
            this.DamageData = new SpecialData(baseAbility, "aura_damage");
        }
    }
}
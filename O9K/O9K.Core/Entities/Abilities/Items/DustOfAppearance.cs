namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.item_dust)]
    public class DustOfAppearance : AreaOfEffectAbility
    {
        public DustOfAppearance(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
        }
    }
}
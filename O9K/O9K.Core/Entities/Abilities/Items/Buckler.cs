namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.item_buckler)]
    public class Buckler : ToggleAbility
    {
        public Buckler(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "bonus_aoe_radius");
        }
    }
}
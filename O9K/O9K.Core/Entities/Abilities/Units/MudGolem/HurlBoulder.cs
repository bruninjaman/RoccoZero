namespace O9K.Core.Entities.Abilities.Units.MudGolem
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.mud_golem_hurl_boulder)]
    public class HurlBoulder : RangedAbility, IDisable
    {
        public HurlBoulder(Ability baseAbility)
            : base(baseAbility)
        {
            this.SpeedData = new SpecialData(baseAbility, "speed");
            this.DamageData = new SpecialData(baseAbility, "damage");
        }

        public UnitState AppliesUnitState { get; } = UnitState.Stunned;
    }
}
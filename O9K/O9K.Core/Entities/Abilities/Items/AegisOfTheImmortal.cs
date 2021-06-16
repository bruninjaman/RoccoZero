namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_aegis)]
    public class AegisOfTheImmortal : PassiveAbility
    {
        public AegisOfTheImmortal(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
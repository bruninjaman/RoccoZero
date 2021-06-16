namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_assault)]
    public class AssaultCuirass : PassiveAbility
    {
        public AssaultCuirass(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
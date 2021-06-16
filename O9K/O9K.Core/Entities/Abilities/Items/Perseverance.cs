namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_pers)]
    public class Perseverance : PassiveAbility
    {
        public Perseverance(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
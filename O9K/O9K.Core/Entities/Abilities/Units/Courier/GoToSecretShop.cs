namespace O9K.Core.Entities.Abilities.Units.Courier
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.courier_go_to_secretshop)]
    public class GoToSecretShop : ActiveAbility
    {
        public GoToSecretShop(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
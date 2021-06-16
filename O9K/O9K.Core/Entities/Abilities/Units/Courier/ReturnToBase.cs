namespace O9K.Core.Entities.Abilities.Units.Courier
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.courier_return_to_base)]
    public class ReturnToBase : ActiveAbility
    {
        public ReturnToBase(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
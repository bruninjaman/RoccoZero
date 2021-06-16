namespace O9K.Core.Entities.Abilities.Heroes.Puck
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.puck_ethereal_jaunt)]
    public class EtherealJaunt : ActiveAbility
    {
        public EtherealJaunt(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
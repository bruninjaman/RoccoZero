namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_echo_sabre)]
    public class EchoSabre : PassiveAbility
    {
        public EchoSabre(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
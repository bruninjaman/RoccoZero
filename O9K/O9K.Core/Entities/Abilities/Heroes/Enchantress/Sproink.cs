namespace O9K.Core.Entities.Abilities.Heroes.Enchantress
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.enchantress_bunny_hop)]
    public class Sproink : ActiveAbility
    {
        public Sproink(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
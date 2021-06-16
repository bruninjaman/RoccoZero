namespace O9K.Core.Entities.Abilities.Heroes.OgreMagi
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.ogre_magi_multicast)]
    public class Multicast : PassiveAbility
    {
        public Multicast(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
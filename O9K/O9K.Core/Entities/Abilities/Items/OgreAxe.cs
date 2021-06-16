namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_ogre_axe)]
    public class OgreAxe : PassiveAbility
    {
        public OgreAxe(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
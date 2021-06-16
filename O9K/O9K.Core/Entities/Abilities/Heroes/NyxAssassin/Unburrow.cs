namespace O9K.Core.Entities.Abilities.Heroes.NyxAssassin
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.nyx_assassin_unburrow)]
    public class Unburrow : ActiveAbility
    {
        public Unburrow(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
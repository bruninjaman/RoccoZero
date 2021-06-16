namespace O9K.Core.Entities.Abilities.Heroes.Pudge
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.pudge_eject)]
    public class Eject : ActiveAbility
    {
        public Eject(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
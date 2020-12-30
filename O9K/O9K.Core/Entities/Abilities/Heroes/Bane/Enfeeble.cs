namespace O9K.Core.Entities.Abilities.Heroes.Bane
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.bane_enfeeble)]
    public class Enfeeble : PassiveAbility
    {
        public Enfeeble(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
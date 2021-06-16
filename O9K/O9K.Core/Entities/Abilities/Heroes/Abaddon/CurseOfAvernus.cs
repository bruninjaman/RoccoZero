namespace O9K.Core.Entities.Abilities.Heroes.Abaddon
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.abaddon_frostmourne)]
    public class CurseOfAvernus : PassiveAbility
    {
        public CurseOfAvernus(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
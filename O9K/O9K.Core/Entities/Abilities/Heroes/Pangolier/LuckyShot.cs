namespace O9K.Core.Entities.Abilities.Heroes.Pangolier
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    //todo id
    [AbilityId((AbilityId)7307)]
    public class LuckyShot : PassiveAbility
    {
        public LuckyShot(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
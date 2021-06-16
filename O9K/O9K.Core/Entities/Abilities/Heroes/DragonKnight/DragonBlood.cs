namespace O9K.Core.Entities.Abilities.Heroes.DragonKnight
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.dragon_knight_dragon_blood)]
    public class DragonBlood : PassiveAbility
    {
        public DragonBlood(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
namespace O9K.Core.Entities.Abilities.Units.AncientBlackDragon
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.black_dragon_splash_attack)]
    public class SplashAttack : PassiveAbility
    {
        public SplashAttack(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
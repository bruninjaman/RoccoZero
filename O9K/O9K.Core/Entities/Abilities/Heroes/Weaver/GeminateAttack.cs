namespace O9K.Core.Entities.Abilities.Heroes.Weaver
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.weaver_geminate_attack)]
    public class GeminateAttack : PassiveAbility
    {
        public GeminateAttack(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
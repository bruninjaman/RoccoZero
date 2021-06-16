namespace O9K.Core.Entities.Abilities.Heroes.ArcWarden
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.arc_warden_scepter)]
    public class RuneForge : ActiveAbility
    {
        public RuneForge(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
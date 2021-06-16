namespace O9K.Core.Entities.Abilities.Heroes.FacelessVoid
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.faceless_void_time_lock)]
    public class TimeLock : PassiveAbility
    {
        public TimeLock(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.Underlord
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.abyssal_underlord_cancel_dark_rift)]
    public class CancelDarkRift : ActiveAbility
    {
        public CancelDarkRift(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
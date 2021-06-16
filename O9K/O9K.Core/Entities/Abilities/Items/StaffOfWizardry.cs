namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_staff_of_wizardry)]
    public class StaffOfWizardry : PassiveAbility
    {
        public StaffOfWizardry(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
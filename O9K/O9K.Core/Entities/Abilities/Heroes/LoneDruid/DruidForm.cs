namespace O9K.Core.Entities.Abilities.Heroes.LoneDruid
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.lone_druid_true_form_druid)]
    public class DruidForm : ActiveAbility
    {
        public DruidForm(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
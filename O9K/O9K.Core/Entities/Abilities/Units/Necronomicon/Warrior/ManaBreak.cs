namespace O9K.Core.Entities.Abilities.Units.Necronomicon.Warrior
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.necronomicon_warrior_mana_burn)]
    public class ManaBreak : PassiveAbility
    {
        public ManaBreak(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
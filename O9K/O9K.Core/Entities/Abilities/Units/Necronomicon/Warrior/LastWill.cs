namespace O9K.Core.Entities.Abilities.Units.Necronomicon.Warrior;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.necronomicon_warrior_last_will)]
public class LastWill : PassiveAbility
{
    public LastWill(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
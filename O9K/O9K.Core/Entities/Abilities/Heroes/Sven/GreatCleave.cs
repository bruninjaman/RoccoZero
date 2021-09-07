namespace O9K.Core.Entities.Abilities.Heroes.Sven;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.sven_great_cleave)]
public class GreatCleave : PassiveAbility
{
    public GreatCleave(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
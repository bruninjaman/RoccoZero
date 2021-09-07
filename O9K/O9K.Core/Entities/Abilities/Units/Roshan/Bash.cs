namespace O9K.Core.Entities.Abilities.Units.Roshan;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.roshan_bash)]
public class Bash : PassiveAbility
{
    public Bash(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
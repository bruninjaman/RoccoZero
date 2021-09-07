namespace O9K.Core.Entities.Abilities.Heroes.Tiny;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.tiny_grow)]
public class Grow : PassiveAbility
{
    public Grow(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
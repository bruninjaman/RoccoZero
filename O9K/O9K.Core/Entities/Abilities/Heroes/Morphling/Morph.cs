namespace O9K.Core.Entities.Abilities.Heroes.Morphling;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.morphling_replicate)]
public class Morph : RangedAbility
{
    public Morph(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
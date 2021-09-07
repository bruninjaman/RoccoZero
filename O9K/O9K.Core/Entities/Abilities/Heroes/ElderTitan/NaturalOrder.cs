namespace O9K.Core.Entities.Abilities.Heroes.ElderTitan;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.elder_titan_natural_order)]
public class NaturalOrder : PassiveAbility
{
    public NaturalOrder(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
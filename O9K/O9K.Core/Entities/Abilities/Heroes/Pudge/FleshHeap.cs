namespace O9K.Core.Entities.Abilities.Heroes.Pudge;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.pudge_flesh_heap)]
public class FleshHeap : PassiveAbility
{
    public FleshHeap(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
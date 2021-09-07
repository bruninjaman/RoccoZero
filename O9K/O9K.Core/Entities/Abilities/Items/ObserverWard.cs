namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_ward_observer)]
public class ObserverWard : RangedAbility
{
    public ObserverWard(Ability ability)
        : base(ability)
    {
    }
}
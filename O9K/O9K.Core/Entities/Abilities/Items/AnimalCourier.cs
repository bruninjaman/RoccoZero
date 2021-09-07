namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_courier)]
public class AnimalCourier : ActiveAbility
{
    public AnimalCourier(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
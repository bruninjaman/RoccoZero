namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_rapier)]
public class DivineRapier : PassiveAbility
{
    public DivineRapier(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
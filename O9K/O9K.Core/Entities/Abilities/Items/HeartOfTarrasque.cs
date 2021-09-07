namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_heart)]
public class HeartOfTarrasque : PassiveAbility
{
    public HeartOfTarrasque(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
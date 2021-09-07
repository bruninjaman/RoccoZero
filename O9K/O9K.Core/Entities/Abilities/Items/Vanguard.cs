namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_vanguard)]
public class Vanguard : PassiveAbility
{
    public Vanguard(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
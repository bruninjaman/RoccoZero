namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_yasha)]
public class Yasha : PassiveAbility
{
    public Yasha(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
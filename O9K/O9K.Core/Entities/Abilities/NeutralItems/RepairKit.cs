namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_repair_kit)]
public class RepairKit : RangedAbility
{
    public RepairKit(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
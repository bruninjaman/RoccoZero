namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_oblivion_staff)]
public class OblivionStaff : PassiveAbility
{
    public OblivionStaff(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_cloak_of_flames)]
public class CloakOfFlames : PassiveAbility
{
    public CloakOfFlames(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
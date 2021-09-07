namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_talisman_of_evasion)]
public class TalismanOfEvasion : PassiveAbility
{
    public TalismanOfEvasion(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
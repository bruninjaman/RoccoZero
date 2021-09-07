namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_ex_machina)]
public class ExMachina : ActiveAbility
{
    public ExMachina(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
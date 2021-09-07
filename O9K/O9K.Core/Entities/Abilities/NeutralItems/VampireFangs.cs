namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_vampire_fangs)]
public class VampireFangs : PassiveAbility
{
    public VampireFangs(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
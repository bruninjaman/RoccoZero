namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_tome_of_knowledge)]
public class TomeOfKnowledge : ActiveAbility
{
    public TomeOfKnowledge(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
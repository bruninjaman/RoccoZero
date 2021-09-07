namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_chainmail)]
public class Chainmail : PassiveAbility
{
    public Chainmail(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
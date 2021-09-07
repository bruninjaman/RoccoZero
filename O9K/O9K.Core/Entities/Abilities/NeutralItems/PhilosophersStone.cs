namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_philosophers_stone)]
public class PhilosophersStone : PassiveAbility
{
    public PhilosophersStone(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
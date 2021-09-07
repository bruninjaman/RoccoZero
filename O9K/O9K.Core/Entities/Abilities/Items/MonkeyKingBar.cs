namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_monkey_king_bar)]
public class MonkeyKingBar : PassiveAbility
{
    public MonkeyKingBar(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
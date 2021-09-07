namespace O9K.Core.Entities.Abilities.Heroes.EarthSpirit;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.earth_spirit_stone_caller)]
public class StoneRemnant : RangedAbility
{
    public StoneRemnant(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
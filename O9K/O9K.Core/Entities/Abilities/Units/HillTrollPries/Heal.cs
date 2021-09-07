namespace O9K.Core.Entities.Abilities.Units.HillTrollPries;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.forest_troll_high_priest_heal)]
public class Heal : RangedAbility
{
    public Heal(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
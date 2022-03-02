namespace O9K.Core.Entities.Abilities.Heroes.PrimalBeast;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.primal_beast_pulverize)]
public class Pulverize : RangedAbility
{
    public Pulverize(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
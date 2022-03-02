namespace O9K.Core.Entities.Abilities.Heroes.PrimalBeast;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.primal_beast_onslaught)]
public class Onslaught : RangedAbility
{
    public Onslaught(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
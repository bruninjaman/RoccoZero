namespace O9K.Core.Entities.Abilities.Heroes.PrimalBeast;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.primal_beast_rock_throw)]
public class RockThrow : CircleAbility
{
    public RockThrow(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
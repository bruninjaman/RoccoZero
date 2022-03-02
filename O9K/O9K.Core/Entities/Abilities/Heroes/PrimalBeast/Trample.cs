namespace O9K.Core.Entities.Abilities.Heroes.PrimalBeast;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.primal_beast_trample)]
public class Trample : ActiveAbility
{
    public Trample(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
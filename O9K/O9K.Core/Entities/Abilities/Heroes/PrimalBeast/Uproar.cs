namespace O9K.Core.Entities.Abilities.Heroes.PrimalBeast;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.primal_beast_uproar)]
public class Uproar : ActiveAbility
{
    public Uproar(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
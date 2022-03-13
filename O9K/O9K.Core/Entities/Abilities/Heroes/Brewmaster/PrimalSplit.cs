namespace O9K.Core.Entities.Abilities.Heroes.Brewmaster;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.brewmaster_primal_split)]
public class PrimalSplit : ActiveAbility
{
    public PrimalSplit(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public override bool IsDisplayingCharges
    {
        get
        {
            return Owner.HasAghanimsScepter;
        }
    }
}
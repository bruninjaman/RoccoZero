namespace O9K.Core.Entities.Abilities.Heroes.Beastmaster;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.beastmaster_inner_beast)]
public class InnerBeast : PassiveAbility
{
    public InnerBeast(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
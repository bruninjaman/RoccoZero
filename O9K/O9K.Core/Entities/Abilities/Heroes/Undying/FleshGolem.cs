namespace O9K.Core.Entities.Abilities.Heroes.Undying;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.undying_flesh_golem)]
public class FleshGolem : ActiveAbility
{
    public FleshGolem(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
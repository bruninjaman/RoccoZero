namespace O9K.Core.Entities.Abilities.Units.Courier;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.courier_shield)]
public class Shield : ActiveAbility
{
    public Shield(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
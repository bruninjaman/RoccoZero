namespace O9K.Core.Entities.Abilities.Heroes.Juggernaut;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.juggernaut_blade_dance)]
public class BladeDance : PassiveAbility
{
    public BladeDance(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
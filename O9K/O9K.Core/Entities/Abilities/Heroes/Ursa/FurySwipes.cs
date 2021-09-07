namespace O9K.Core.Entities.Abilities.Heroes.Ursa;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.ursa_fury_swipes)]
public class FurySwipes : PassiveAbility
{
    public FurySwipes(Ability ability)
        : base(ability)
    {
    }
}
namespace O9K.Core.Entities.Abilities.Heroes.Doom;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.doom_bringer_infernal_blade)]
public class InfernalBlade : OrbAbility, IHarass
{
    public InfernalBlade(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
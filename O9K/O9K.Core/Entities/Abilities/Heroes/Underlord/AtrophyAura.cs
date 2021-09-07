namespace O9K.Core.Entities.Abilities.Heroes.Underlord;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.abyssal_underlord_atrophy_aura)]
public class AtrophyAura : PassiveAbility
{
    public AtrophyAura(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
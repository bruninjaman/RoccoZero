namespace O9K.Core.Entities.Abilities.Units.WildwingReaper;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.enraged_wildkin_toughness_aura)]
public class ToughnessAura : PassiveAbility
{
    public ToughnessAura(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
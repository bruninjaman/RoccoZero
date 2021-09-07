namespace O9K.Core.Entities.Abilities.Heroes.LoneDruid.SpiritBear;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.lone_druid_spirit_bear_demolish)]
public class Demolish : PassiveAbility
{
    public Demolish(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
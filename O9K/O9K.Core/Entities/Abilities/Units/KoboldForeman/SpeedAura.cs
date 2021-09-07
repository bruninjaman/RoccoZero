namespace O9K.Core.Entities.Abilities.Units.KoboldForeman;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.kobold_taskmaster_speed_aura)]
public class SpeedAura : PassiveAbility
{
    public SpeedAura(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
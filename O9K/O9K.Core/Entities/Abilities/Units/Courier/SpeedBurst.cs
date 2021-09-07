namespace O9K.Core.Entities.Abilities.Units.Courier;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.courier_burst)]
public class SpeedBurst : ActiveAbility
{
    public SpeedBurst(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
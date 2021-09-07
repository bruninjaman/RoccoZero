namespace O9K.Core.Entities.Abilities.Heroes.Io;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.wisp_tether)]
public class Tether : RangedAbility
{
    public Tether(Ability baseAbility)
        : base(baseAbility)
    {
        this.SpeedData = new SpecialData(baseAbility, "latch_speed");
    }
}
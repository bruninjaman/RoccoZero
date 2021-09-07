namespace O9K.Core.Entities.Abilities.Heroes.Razor;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.razor_unstable_current)]
public class StormSurge : PassiveAbility
{
    public StormSurge(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
    }
}
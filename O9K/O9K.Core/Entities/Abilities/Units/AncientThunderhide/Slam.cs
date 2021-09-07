namespace O9K.Core.Entities.Abilities.Units.AncientThunderhide;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.big_thunder_lizard_slam)]
public class Slam : AreaOfEffectAbility, IHarass
{
    public Slam(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
    }
}
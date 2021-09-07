namespace O9K.Core.Entities.Abilities.Heroes.NaturesProphet;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.furion_force_of_nature)]
public class NaturesCall : CircleAbility
{
    public NaturesCall(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "area_of_effect");
    }
}
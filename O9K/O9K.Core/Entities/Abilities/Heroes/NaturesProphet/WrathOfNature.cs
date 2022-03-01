namespace O9K.Core.Entities.Abilities.Heroes.NaturesProphet;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.furion_wrath_of_nature)]
public class WrathOfNature : RangedAbility, INuke
{
    public WrathOfNature(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "damage");
    }

    public override float CastRange { get; } = 9999999;
}
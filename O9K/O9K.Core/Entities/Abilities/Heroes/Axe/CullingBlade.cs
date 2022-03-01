namespace O9K.Core.Entities.Abilities.Heroes.Axe;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.axe_culling_blade)]
public class CullingBlade : RangedAbility, INuke
{
    public CullingBlade(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, AbilityId.special_bonus_unique_axe_5, "damage");
    }

    protected override float BaseCastRange
    {
        get
        {
            return base.BaseCastRange + 100;
        }
    }
}
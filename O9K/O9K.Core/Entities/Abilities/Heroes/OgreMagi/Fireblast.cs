namespace O9K.Core.Entities.Abilities.Heroes.OgreMagi;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.ogre_magi_fireblast)]
[AbilityId(AbilityId.ogre_magi_unrefined_fireblast)]
public class Fireblast : RangedAbility, IDisable, INuke
{
    public Fireblast(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "fireblast_damage");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}
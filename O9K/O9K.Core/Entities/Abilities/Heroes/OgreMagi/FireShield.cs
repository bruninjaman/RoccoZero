namespace O9K.Core.Entities.Abilities.Heroes.OgreMagi;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.ogre_magi_smash)]
public class FireShield : RangedAbility, IShield
{
    public FireShield(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = UnitState.None;

    public string ShieldModifierName { get; } = "modifier_ogre_magi_smash_buff";

    public bool ShieldsAlly { get; } = true;

    public bool ShieldsOwner { get; } = true;
}
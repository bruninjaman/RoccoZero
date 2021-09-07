namespace O9K.Core.Entities.Abilities.Heroes.Tinker;

using Base;
using Base.Components;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Entities.Units;

using Helpers;

using Metadata;

[AbilityId(AbilityId.tinker_defense_matrix)]
public class DefenseMatrix : RangedAbility, IShield, IHasDamageBlock
{

    public DefenseMatrix(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "damage_absorb");
    }

    public string ShieldModifierName { get; } = "modifier_tinker_defense_matrix";

    public bool ShieldsAlly { get; } = true;

    public bool ShieldsOwner { get; } = true;

    public DamageType BlockDamageType { get; } = DamageType.Physical | DamageType.Magical | DamageType.Pure;

    public UnitState AppliesUnitState { get; } = 0;

    public bool ProvidesStatusResistance
    {
        get
        {
            return true;
        }
    }

    public override bool TargetsEnemy { get; } = false;

    public string BlockModifierName { get; } = "modifier_tinker_defense_matrix";

    public bool BlocksDamageAfterReduction { get; } = false;

    public bool IsDamageBlockPermanent { get; } = false;

    public float BlockValue(Unit9 target)
    {
        return this.DamageData.GetValue(this.Level);
    }
}
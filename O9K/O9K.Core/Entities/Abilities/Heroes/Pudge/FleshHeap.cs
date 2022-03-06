namespace O9K.Core.Entities.Abilities.Heroes.Pudge;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

using O9K.Core.Entities.Abilities.Base.Components;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Units;
using O9K.Core.Helpers;

[AbilityId(AbilityId.pudge_flesh_heap)]
public class FleshHeap : ActiveAbility, IHasDamageBlock, IShield
{
    private readonly SpecialData blockData;

    public FleshHeap(Ability baseAbility)
        : base(baseAbility)
    {
        this.blockData = new SpecialData(baseAbility, "damage_block");
    }

    public DamageType BlockDamageType { get; } = DamageType.Physical | DamageType.Magical | DamageType.Pure;

    public string BlockModifierName { get; } = "modifier_pudge_flesh_heap_block";

    public bool BlocksDamageAfterReduction { get; } = false;

    public bool IsDamageBlockPermanent { get; } = false;

    public UnitState AppliesUnitState { get; } = UnitState.None;

    public string ShieldModifierName { get; } = "modifier_pudge_flesh_heap_block";

    public bool ShieldsAlly { get; } = false;

    public bool ShieldsOwner { get; } = true;

    public float BlockValue(Unit9 target)
    {
        return this.blockData.GetValue(this.Level);
    }
}
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Metadata;

[AbilityId(AbilityId.item_force_field)]
public class ArcanistsArmor : AreaOfEffectAbility, IShield
{
    public ArcanistsArmor(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = 0;

    public string ShieldModifierName { get; } = "modifier_item_force_field_bonus_aura";

    public bool ShieldsAlly { get; } = true;

    public bool ShieldsOwner { get; } = true;
}
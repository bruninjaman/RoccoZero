namespace O9K.Core.Entities.Abilities.Heroes.Axe;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.axe_battle_hunger)]
public class BattleHunger : RangedAbility, IDebuff
{
    public BattleHunger(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public string DebuffModifierName { get; } = "modifier_axe_battle_hunger";
}
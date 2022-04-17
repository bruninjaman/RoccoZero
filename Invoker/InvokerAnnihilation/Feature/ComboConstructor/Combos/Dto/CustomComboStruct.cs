using Divine.Entity.Entities.Abilities.Components;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Feature.ComboConstructor.Combos.Dto;

public class CustomComboStruct
{
    public readonly IAbility? Ability;

    public CustomComboStruct(IAbility? ability)
    {
        Ability = ability;
        if (ability != null) AbilityId = ability.AbilityId;
    }
    public CustomComboStruct(AbilityId abilityId)
    {
        Ability = null;
        AbilityId = abilityId;
    }

    public AbilityId AbilityId { get; set; }
    public RectangleF Position { get; set; }

    public bool IsHover { get; set; }
}
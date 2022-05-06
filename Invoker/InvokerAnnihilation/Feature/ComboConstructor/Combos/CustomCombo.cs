using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.AbilityManager;
using InvokerAnnihilation.Feature.ComboConstructor.Combos.Dto;

namespace InvokerAnnihilation.Feature.ComboConstructor.Combos;

public class CustomCombo : ComboBase
{
    public CustomCombo(IAbilityManager abilityManager, int maxAbilities) : base(abilityManager)
    {
        Abilities = new Dictionary<int, CustomComboStruct>();
        for (int i = 0; i < maxAbilities; i++)
        {
            Abilities.Add(i, new CustomComboStruct(AbilityId.dota_base_ability));
        }
    }
}
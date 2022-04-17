using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.AbilityManager;
using InvokerAnnihilation.Feature.ComboConstructor.Combos.Dto;

namespace InvokerAnnihilation.Feature.ComboConstructor.Combos;

public class StandardCombo : ComboBase
{
    public StandardCombo(IAbilityManager abilityManager, params AbilityId[] abilityIds) : base(abilityManager)
    {
        for (var index = 0; index < abilityIds.Length; index++)
        {
            var id = abilityIds[index];
            Abilities[index] = new CustomComboStruct(abilityManager.GetAbility(id));
        }
    }

    public int ReqQuasLevel { get; set; } = 0;
    public int ReqWexLevel { get; set; } = 0;
    public int ReqExortLevel { get; set; } = 0;
    public List<AbilityId> ItemsToHave { get; set; } = new();
}
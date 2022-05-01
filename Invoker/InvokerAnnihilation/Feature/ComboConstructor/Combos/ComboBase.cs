using Divine.Entity.Entities.Abilities.Components;
using Divine.Numerics;
using InvokerAnnihilation.Abilities.AbilityManager;
using InvokerAnnihilation.Feature.ComboConstructor.Combos.Dto;
using InvokerAnnihilation.Feature.ComboConstructor.Interface;

namespace InvokerAnnihilation.Feature.ComboConstructor.Combos;

public abstract class ComboBase : ICombo
{
    protected readonly IAbilityManager AbilityManager;

    protected ComboBase(IAbilityManager abilityManager)
    {
        AbilityManager = abilityManager;
        Abilities = new Dictionary<int, CustomComboStruct>();
        for (int i = 0; i < 15; i++)
        {
            Abilities.Add(i, new CustomComboStruct(AbilityId.dota_base_ability));
        }
        ChangeIndex = -1;
        IsActive = false;
        ActivateBtnPosition = RectangleF.Empty;
    }

    public Dictionary<int, CustomComboStruct> Abilities { get; set; }

    public Dictionary<int, CustomComboStruct> ValidAbilities =>
        Abilities.Where(x => x.Value.AbilityId != AbilityId.dota_base_ability).ToDictionary(x => x.Key, z => z.Value);

    public int ChangeIndex { get; set; }
    public RectangleF ActivateBtnPosition { get; set; }
    public bool IsActive { get; set; }
}
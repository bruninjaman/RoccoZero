using Divine.Entity.Entities.Abilities.Components;
using InvokerAnnihilation.Abilities.AbilityManager;
using InvokerAnnihilation.Feature.ComboConstructor.Combos.Dto;
using InvokerAnnihilation.Feature.ComboConstructor.Interface;

namespace InvokerAnnihilation.Feature.ComboConstructor.Combos.Factory;

public class ComboFactory : IComboFactory<StandardCombo>
{
    private readonly IAbilityManager _abilityManager;
    private StandardCombo StandardCombo { get; init; }

    public ComboFactory(IAbilityManager abilityManager)
    {
        _abilityManager = abilityManager;
        StandardCombo = new StandardCombo(abilityManager);
    }

    public IComboFactory<StandardCombo?> SetAbilities(params AbilityId[] abilityIds)
    {
        for (var index = 0; index < abilityIds.Length; index++)
        {
            var id = abilityIds[index];
            StandardCombo.Abilities[index] = new CustomComboStruct(_abilityManager.GetAbility(id));
        }
        // StandardCombo.Abilities =
        //     abilityIds.Select((x, i) => new {Item = x, Index = i})
        //         .ToDictionary(x => x.Index, x => new CustomComboStruct(x.Item));
        return this;
    }

    public IComboFactory<StandardCombo> SetItemsToHave(params AbilityId[] abilityIds)
    {
        StandardCombo.ItemsToHave = abilityIds.ToList();
        return this;
    }

    public IComboFactory<StandardCombo> SetQuasLevel(int level)
    {
        StandardCombo.ReqQuasLevel = level;
        return this;
    }

    public IComboFactory<StandardCombo> SetWexLevel(int level)
    {
        StandardCombo.ReqWexLevel = level;
        return this;
    }

    public IComboFactory<StandardCombo> SetExortLevel(int level)
    {
        StandardCombo.ReqExortLevel = level;
        return this;
    }

    public StandardCombo Build()
    {
        return StandardCombo;
    }
}
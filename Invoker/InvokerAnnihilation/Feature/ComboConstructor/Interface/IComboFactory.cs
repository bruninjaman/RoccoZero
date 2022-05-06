using Divine.Entity.Entities.Abilities.Components;

namespace InvokerAnnihilation.Feature.ComboConstructor.Interface;

public interface IComboFactory<out T> where T: ICombo
{
    IComboFactory<T> SetAbilities(params AbilityId[] abilityIds);
    IComboFactory<T> SetItemsToHave(params AbilityId[] abilityIds);
    IComboFactory<T> SetQuasLevel(int level);
    IComboFactory<T> SetWexLevel(int level);
    IComboFactory<T> SetExortLevel(int level);
    T Build();
}
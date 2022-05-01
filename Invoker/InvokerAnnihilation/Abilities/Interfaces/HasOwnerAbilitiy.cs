using Divine.Entity.Entities.Abilities.Components;

namespace InvokerAnnihilation.Abilities.Interfaces;

public interface IHasOwnerAbility
{
    public AbilityId OwnerAbility { get; set; }
}
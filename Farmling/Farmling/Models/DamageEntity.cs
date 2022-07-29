using Farmling.CreepAbilities.Base;

namespace Farmling.Models;

public record DamageEntity
{
    public DamageEntityType EntityType { get; init; }
    public BaseDamageModifierAbility Ability { get; set; }
}

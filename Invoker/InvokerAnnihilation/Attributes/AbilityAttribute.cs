using Divine.Entity.Entities.Abilities.Components;

namespace InvokerAnnihilation.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AbilityAttribute : Attribute
{
    public AbilityId[] Spheres { get; } = Array.Empty<AbilityId>();
    public AbilityId[] AbilityIds { get; }

    public AbilityAttribute(AbilityId abilityId)
    {
        AbilityIds = new[] {abilityId};
    }
    public AbilityAttribute(AbilityId abilityId, AbilityId[] spheres)
    {
        Spheres = spheres;
        AbilityIds = new[] {abilityId};
    }

    public AbilityAttribute(params AbilityId[] abilityIds)
    {
        AbilityIds = abilityIds;
    }
}


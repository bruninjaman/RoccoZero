namespace O9K.Core.Entities.Abilities.Heroes.Bane;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.bane_enfeeble)]
public class Enfeeble : PassiveAbility
{
    public Enfeeble(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
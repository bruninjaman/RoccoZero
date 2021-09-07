namespace O9K.Core.Entities.Abilities.Heroes.Brewmaster.Spirits;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.brewmaster_fire_permanent_immolation)]
public class PermanentImmolation : PassiveAbility
{
    public PermanentImmolation(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
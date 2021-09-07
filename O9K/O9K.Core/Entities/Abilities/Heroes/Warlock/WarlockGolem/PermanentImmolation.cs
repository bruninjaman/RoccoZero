namespace O9K.Core.Entities.Abilities.Heroes.Warlock.WarlockGolem;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.warlock_golem_permanent_immolation)]
public class PermanentImmolation : PassiveAbility
{
    public PermanentImmolation(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
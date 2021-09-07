namespace O9K.Core.Entities.Abilities.Heroes.Visage;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.visage_summon_familiars)]
public class SummonFamiliars : ActiveAbility
{
    public SummonFamiliars(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
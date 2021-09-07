namespace O9K.Core.Entities.Abilities.Heroes.Slark;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.slark_essence_shift)]
public class EssenceShift : PassiveAbility
{
    public EssenceShift(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
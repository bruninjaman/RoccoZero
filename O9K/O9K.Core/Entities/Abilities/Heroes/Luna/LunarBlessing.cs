namespace O9K.Core.Entities.Abilities.Heroes.Luna;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.luna_lunar_blessing)]
public class LunarBlessing : PassiveAbility
{
    public LunarBlessing(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
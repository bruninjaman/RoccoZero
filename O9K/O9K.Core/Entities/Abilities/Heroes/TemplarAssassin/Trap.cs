namespace O9K.Core.Entities.Abilities.Heroes.TemplarAssassin;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.templar_assassin_trap)]
public class Trap : ActiveAbility
{
    public Trap(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
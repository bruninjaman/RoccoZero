namespace O9K.Core.Entities.Abilities.Heroes.ElderTitan;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.elder_titan_move_spirit)]
public class MoveAstralSpirit : ActiveAbility
{
    public MoveAstralSpirit(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
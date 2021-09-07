namespace O9K.Core.Entities.Abilities.Heroes.Windranger;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.windrunner_focusfire)]
public class FocusFire : RangedAbility
{
    public FocusFire(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
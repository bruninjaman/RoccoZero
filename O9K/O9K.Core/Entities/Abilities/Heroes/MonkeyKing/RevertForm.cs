namespace O9K.Core.Entities.Abilities.Heroes.MonkeyKing;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.monkey_king_untransform)]
public class RevertForm : ActiveAbility
{
    public RevertForm(Ability ability)
        : base(ability)
    {
    }
}
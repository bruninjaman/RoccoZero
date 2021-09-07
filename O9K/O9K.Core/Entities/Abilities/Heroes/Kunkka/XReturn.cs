namespace O9K.Core.Entities.Abilities.Heroes.Kunkka;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.kunkka_return)]
public class XReturn : ActiveAbility
{
    public XReturn(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
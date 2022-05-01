namespace O9K.Core.Entities.Abilities.Heroes.Brewmaster;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.brewmaster_drunken_brawler)]
public class DrunkenBrawler : ToggleAbility
{
    public DrunkenBrawler(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
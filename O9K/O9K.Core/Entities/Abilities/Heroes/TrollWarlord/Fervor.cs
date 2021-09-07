namespace O9K.Core.Entities.Abilities.Heroes.TrollWarlord;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.troll_warlord_fervor)]
public class Fervor : PassiveAbility
{
    public Fervor(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
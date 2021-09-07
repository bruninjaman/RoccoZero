namespace O9K.Core.Entities.Abilities.Units.Necronomicon.Archer;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.necronomicon_archer_aoe)]
public class ArcherAura : PassiveAbility
{
    public ArcherAura(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
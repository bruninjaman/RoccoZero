namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_octarine_core)]
public class OctarineCore : PassiveAbility
{
    public OctarineCore(Ability baseAbility)
        : base(baseAbility)
    {
    }
}
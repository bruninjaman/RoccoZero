﻿namespace O9K.ItemManager.Modules.RecoveryAbuse.Abilities;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;

[AbilityId(AbilityId.item_soul_ring)]
internal class SoulRing : RecoveryAbility
{
    public SoulRing(Ability9 ability)
        : base(ability)
    {
    }

    public override Attribute PowerTreadsAttribute { get; } = Attribute.Strength;
}
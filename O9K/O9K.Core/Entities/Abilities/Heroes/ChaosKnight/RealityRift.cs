﻿namespace O9K.Core.Entities.Abilities.Heroes.ChaosKnight;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.chaos_knight_reality_rift)]
public class RealityRift : RangedAbility
{
    public RealityRift(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public override bool CanHitSpellImmuneEnemy
    {
        get
        {
            if (this.Owner.GetAbilityById(AbilityId.special_bonus_unique_chaos_knight)?.Level > 0)
            {
                return true;
            }

            return base.CanHitSpellImmuneEnemy;
        }
    }
}